using System.Diagnostics;
namespace Backend;

public class DockerBuilder
{
    private string working_directory;
    private Language language;
    private string? filename;
    private string[] dockerLines;

    public DockerBuilder(Language language, string working_directory)
    {
        this.working_directory = working_directory;
        this.language = language;
        switch (language)
        {
            case Language.python:
                this.filename = "main.py";
                this.dockerLines = new string[] { "FROM python:latest", $"COPY {filename} /", $"CMD [ \"python\", \"./{filename}\" ]" };
                break;
            case Language.dotnet:
                this.filename = "Program.cs";
                this.dockerLines = new string[] { "FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env", "WORKDIR /app", $"COPY . ./", "RUN dotnet restore", "RUN dotnet publish -c Release -o out",
                "FROM mcr.microsoft.com/dotnet/aspnet:6.0", "WORKDIR /app", "COPY --from=build-env /app/out .", "ENTRYPOINT [\"dotnet\", \"dotnet.dll\"]" };
                break;
            default:
                this.filename = "";
                this.dockerLines = new string[] { "" };
                throw new FileLoadException("Error creating new files", nameof(language));
        }
    }

    public string createDockerFile()
    {

        /// uncomment to create template file on demand. 
        // string[] filelines = { "#!/usr/bin/env python3", "", "print(\"hello world\")" };
        // using (StreamWriter codeFile = new StreamWriter(Path.Combine(working_directory, filename)))
        // {
        //     foreach (string line in filelines)
        //         codeFile.WriteLine(line);
        // }

        // Write the string array to a new file named "Dockerfile".
        try
        {
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(working_directory, $"Dockerfile")))
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
    public string runDockerFile()
    {
        Process p = new Process();
        // Redirect the output stream of the child process.
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.RedirectStandardOutput = true;
        p.StartInfo.FileName = "templates/rundockerfile.sh";
        p.StartInfo.Arguments = $"{working_directory} runtime-session";
        p.Start();

        // returns false if the program has not finished in n seconds
        bool checkexit = p.WaitForExit(60000);
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

    public void addTemplateFiles()
    {
        switch (language)
        {
            case Language.python:
                File.Copy($"templates/{filename}", $"{working_directory}/{filename}");
                break;
            case Language.dotnet:
                string sourcepath = "templates/dotnet";
                string targetpath = $"{working_directory}";
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
                break;
            default:
                throw new FileLoadException("Error creating new files", nameof(language));
        }
    }
}
