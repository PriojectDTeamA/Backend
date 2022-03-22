namespace Backend;

public class DockerSession
{
    public Language language { get; set; }
    public string sessionName { get; set; }
    readonly string workingDirectory;
    private DockerBuilder builder;

    public DockerSession(Language language, string sessionName)
    {
        this.language = language;
        this.sessionName = sessionName;
        this.workingDirectory = "./sessions/" + sessionName + "/";
        this.builder = new DockerBuilder(language, workingDirectory);
        this.createSessionDirectory();
    }

    public void build()
    {
        builder.createDockerFile();
    }

    public string run()
    {
        return builder.runDockerFile();
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
