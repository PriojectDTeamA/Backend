namespace Backend;

public class DockerSession
{
    public Language language { get; set; }
    public string sessionName { get; set; }
    readonly string workingDirectory;
    private DockerBuilder builder;

    // public DockerSession(Language language, string sessionName)
    // {
    //     this.language = language;
    //     this.sessionName = sessionName;
    //     this.workingDirectory = "./sessions/" + sessionName + "/";
    //     this.builder = new DockerBuilder(language, workingDirectory);
    //     this.createSessionDirectory();
    // }

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
            builder.addTemplateFiles();
        }
    }
}

public class PythonSession : DockerSession
{
    public PythonSession(string sessionName) : base(sessionName, new PythonBuilder(sessionName))
    {

    }

    public override void build()
    {
        base.build();
    }
}

public class DotnetSession : DockerSession
{
    public DotnetSession(string sessionName) : base(sessionName, new DotnetBuilder(sessionName))
    {

    }
}