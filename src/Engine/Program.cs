using CommandLine;
using Engine;
using Octokit;
using Octokit.Internal;

const string owner = "MikeAmputer";
const string repositoryName = "orcs-have-issues";
const string version = "1";

var productInfo = new ProductHeaderValue(owner, version);
var options = Parser.Default.ParseArguments<Options>(args).Value;
var credentialStore = new InMemoryCredentialStore(new Credentials(options.GitHubToken));
var since = DateTimeOffset.UtcNow.AddHours(-options.PeriodHours);


var ghClient = new GitHubClient(productInfo, credentialStore);

var repository = await ghClient.Repository.Get(owner, repositoryName);

Logging.RateLimitProvider = () => ghClient.GetLastApiInfo()?.RateLimit;

var issueRepository = await IssueRepository.Create(repository, ghClient.Issue, since);

var characters = issueRepository.GetCharacters().ToList();

Logging.LogInfo("Start fight");

var result = ActionRunner.Execute("fight goblin-camp 3", characters[0]);

Logging.LogInfo("End fight");

Logging.LogInfo("Complete");
