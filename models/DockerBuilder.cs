using System.Diagnostics;
namespace Backend;

public class DockerBuilder
{
    public static String working_directory = "./sessions/" + new Random().Next().ToString() + "/";

    public static string createDockerFile()
    {
        string filename = "main.py";
        string[] filelines = { "#!/usr/bin/env python3", "", "print(\"hello world\")" };
        string[] lines = { "FROM python:latest", $"COPY {filename} /", $"CMD [ \"python\", \"./{filename}\" ]" };
        if (!Directory.Exists(working_directory))
        {
            Directory.CreateDirectory(working_directory);
        }

        // using (StreamWriter codeFile = new StreamWriter(Path.Combine(working_directory, filename)))
        // {
        //     foreach (string line in filelines)
        //         codeFile.WriteLine(line);
        // }

        File.Copy($"templates/{filename}", $"{working_directory}/{filename}");

        // Write the string array to a new file named "WriteLines.txt".
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
        Console.WriteLine(p.WaitForExit(1000));

        // Read the output stream first and then wait.
        string output = p.StandardOutput.ReadToEnd();
        System.Console.WriteLine(output);

        // TODO: exit na 10 seconden
        p.WaitForExit();
        return output;
    }
}