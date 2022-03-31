using System.Diagnostics;
using System.IO;
namespace Backend;

public abstract class DockerBuilder
{
    private string[] dockerLines;

    public DockerBuilder(string[] dockerLines)
    {
        this.dockerLines = dockerLines;
    }

    public virtual string createDockerFile(string dir)
    {
        // Write the string array to a new file named "Dockerfile".
        try
        {
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(dir, $"Dockerfile")))
            {
                foreach (string line in dockerLines)
                    outputFile.WriteLine(line);
            }
        }
        catch (System.Exception e)
        {
            System.Console.WriteLine(e);
            throw;
        }
        return " ";
    }
    public virtual string runDockerFile(string dir)
    {
        Process p = new Process();
        // Redirect the output stream of the child process.
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.RedirectStandardOutput = true;
        p.StartInfo.FileName = "templates/rundockerfile.sh";
        p.StartInfo.Arguments = $"{dir} runtime-session";
        p.Start();

        // returns false if the program has not finished in n seconds
        bool checkexit = p.WaitForExit(600000);
        // checks if the program is still running after n seconds and forcefully terminates it if it is still running.
        if (!checkexit)
        {
            System.Console.WriteLine("Program terminating early!");
            p.Kill(true);
            return "No result, program took to long to run...";
        }

        // Read the output stream first and then wait.
        string output = p.StandardOutput.ReadToEnd();
        System.Console.WriteLine(output);

        // cleaning up the output, currently using a manual print statement in the template file.
        try
        {
            String cleanedOutput = output.Split("Printing output:", 2)[1];
            return cleanedOutput;
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e + "\nThere looks to be a problem splitting the output. Check if docker is behaving correctly");
        }
        return output;
    }

    public virtual void addTemplateFiles(string dir)
    {
        return;
    }
    public void CopyDir(string sourceFolder, string destFolder)
    {
        if (!Directory.Exists(destFolder))
            Directory.CreateDirectory(destFolder);

        // Get Files & Copy
        string[] files = Directory.GetFiles(sourceFolder);
        foreach (string file in files)
        {
            string name = Path.GetFileName(file);

            // ADD Unique File Name Check to Below!!!!
            string dest = Path.Combine(destFolder, name);
            File.Copy(file, dest);
        }

        // Get dirs recursively and copy files
        string[] folders = Directory.GetDirectories(sourceFolder);
        foreach (string folder in folders)
        {
            string name = Path.GetFileName(folder);
            string dest = Path.Combine(destFolder, name);
            CopyDir(folder, dest);
        }
    }
}

public class PythonBuilder : DockerBuilder
{
    string filename = "main.py";
    string[] dockerLines = { "FROM python:latest", $"COPY main.py /", $"CMD [ \"python\", \"./main.py\" ]" };
    // string working_directory;
    public PythonBuilder(string sessionName) : base(new string[] {
        "FROM python:latest",
        $"COPY main.py /",
        $"CMD [ \"python\", \"./main.py\" ]" })
    {
    }

    public override void addTemplateFiles(string dir)
    {
        File.Copy($"templates/python/{filename}", $"{dir}/{filename}");
    }
}

public class DotnetBuilder : DockerBuilder
{
    public DotnetBuilder(string sessionName) : base(new string[] {
        "FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env",
        "WORKDIR /app",
        $"COPY . ./",
        "RUN dotnet restore",
        "RUN dotnet publish -c Release -o out",
        "FROM mcr.microsoft.com/dotnet/aspnet:6.0",
        "WORKDIR /app",
        "COPY --from=build-env /app/out .",
        "ENTRYPOINT [\"dotnet\", \"dotnet.dll\"]"})
    {
    }

    public override void addTemplateFiles(string dir)
    {
        string sourcepath = "templates/dotnet";
        string targetpath = $"{dir}";
        string[] files = System.IO.Directory.GetFiles(sourcepath);

        // Copy the files and overwrite destination files if they already exist.
        foreach (string s in files)
        {
            // Use static Path methods to extract only the file name from the path.
            string fileName = System.IO.Path.GetFileName(s);
            if (fileName == "Program.csx")
            {
                Path.ChangeExtension(fileName, ".cs");
                fileName = "Program.cs";
            }
            string destFile = System.IO.Path.Combine(targetpath, fileName);
            System.IO.File.Copy(s, destFile, true);
        }
    }
}

public class JavascriptBuilder : DockerBuilder
{
    public JavascriptBuilder(string sessionName) : base(new string[] {
        "FROM node:16",
        "COPY package*.json ./",
        "RUN npm install",
        "COPY . .",
        "CMD [ \"node\", \".\" ]",
    })
    {

    }

    public override void addTemplateFiles(string dir)
    {
        string sourcepath = "templates/javascript";
        string targetpath = $"{dir}";
        base.CopyDir(sourcepath, targetpath);
    }
}


