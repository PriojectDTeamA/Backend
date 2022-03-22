using System.Diagnostics;
namespace Backend;

public class DockerBuilder
{
    public static String working_directory = "./sessions/" + new Random().Next().ToString() + "/";

    public static string createDockerFile()
    // currently only supported for python
    {
        // create a new directory for the current session
        if (!Directory.Exists(working_directory))
        {
            Directory.CreateDirectory(working_directory);
        }

        /// uncomment to create template file on demand. 
        // string[] filelines = { "#!/usr/bin/env python3", "", "print(\"hello world\")" };
        // using (StreamWriter codeFile = new StreamWriter(Path.Combine(working_directory, filename)))
        // {
        //     foreach (string line in filelines)
        //         codeFile.WriteLine(line);
        // }

        string filename = "main.py";
        string[] lines = { "FROM python:latest", $"COPY {filename} /", $"CMD [ \"python\", \"./{filename}\" ]" };

        // copy the template file to our working directory
        File.Copy($"templates/{filename}", $"{working_directory}/{filename}", true);

        // Write the string array to a new file named "Dockerfile".
        try
        {
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(working_directory, $"Dockerfile")))
            {
                foreach (string line in lines)
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
    public static string runDockerFile()
    {
        Process p = new Process();
        // Redirect the output stream of the child process.
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.RedirectStandardOutput = true;
        p.StartInfo.FileName = "templates/rundockerfile.sh";
        p.StartInfo.Arguments = $"{working_directory} runtime-session";
        p.Start();

        // returns false if the program has not finished in n seconds
        bool checkexit = p.WaitForExit(10000);
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
}
