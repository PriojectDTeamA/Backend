namespace Backend;

public abstract class DockerSession
{
    public Language language { get; set; }
    public string sessionName { get; set; }
    readonly string workingDirectory;
    private DockerBuilder builder;

    public DockerSession(string sessionName, DockerBuilder builder)
    {
        this.workingDirectory = "./sessions/" + sessionName + "/";
        this.sessionName = sessionName;
        this.builder = builder;
        this.createSessionDirectory();
    }

    public virtual void build()
    {
        builder.createDockerFile(workingDirectory);
    }

    public string run()
    {
        return builder.runDockerFile(workingDirectory);
    }

    private bool firstRun()
    {
        if (!Directory.Exists(workingDirectory))
        {
            return true;
        }
        return false;
    }

    private void createSessionDirectory()
    {
        if (firstRun())
        {
            Directory.CreateDirectory(workingDirectory);
            builder.addTemplateFiles(workingDirectory);
        }
    }

    private string getFileInfo()
    {

        return "";
    }
}

public class PythonSession : DockerSession
{
    public PythonSession(string sessionName) : base(sessionName, new PythonBuilder(sessionName))
    {

    }
}

public class DotnetSession : DockerSession
{
    public DotnetSession(string sessionName) : base(sessionName, new DotnetBuilder(sessionName))
    {

    }
}

public class JavascriptSession : DockerSession
{
    public JavascriptSession(string sessionName) : base(sessionName, new JavascriptBuilder(sessionName))
    {

    }
}