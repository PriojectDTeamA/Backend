using System.Diagnostics;
namespace Backend;

public class DockerBuilder
{
    public static String working_directory = "./sessions/" + new Random().Next().ToString() + "/";

    public static string createDockerFile()
    {
        string filename = "main.py";
        string[] lines = { "FROM python:latest", $"COPY {filename} /", $"CMD [ \"python\", ./{filename} ]" };
        if (!Directory.Exists(working_directory))
        {
            Directory.CreateDirectory(working_directory);
        }

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
        p.StartInfo.FileName = "rundockerfile.sh";
        p.Start();
        // Do not wait for the child process to exit before
        // reading to the end of its redirected stream.
        // p.WaitForExit();
        // Read the output stream first and then wait.
        string output = p.StandardOutput.ReadToEnd();
        System.Console.WriteLine(output);
        System.Console.WriteLine(p.WaitForExit(10));
        return output;
    }
}