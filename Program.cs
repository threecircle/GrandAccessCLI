// https://docs.microsoft.com/ru-ru/dotnet/standard/commandline/

using System.CommandLine;
using System.CommandLine.Invocation;
using GrantAccessCLI;
using System.Text.Json;
using System.Text.Json.Serialization;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using System.Text;

ContextModel _context = JsonSerializer.Deserialize<ContextModel>(File.ReadAllText("context.json"));

var cmd = new RootCommand("Command Line Interface for grant access manager");
var cmd_outFormatOpt = new Option<OutputFormat>(name: "--out-format", description: "Output format");

#region APPLY
var applyCmd = new Command("apply", "Apply manifest file");

var applyCmd_FileArg = new Argument<FileInfo>("file", "Путь до файла с манифестом");

applyCmd.Add(applyCmd_FileArg);

applyCmd.SetHandler(
    (FileInfo file) =>
    {
        
        //Read manifest
        string manifest = File.ReadAllText(file.FullName);
        var deserializer = new DeserializerBuilder().Build();
        var obj = deserializer.Deserialize<ManifestModel>(manifest);

        //Create request
        string payload = JsonSerializer.Serialize(obj);

        HttpClient client = new HttpClient();
        var response = client.PostAsync(_context.server.ToString() + "api/" + obj.apiVersion + "/" + obj.kind, new StringContent(payload, Encoding.UTF8, "application/json")).Result;

        Console.WriteLine(response.StatusCode.ToString());

    }, applyCmd_FileArg);
#endregion

#region context
var contextCmd = new Command("set-context", "Set connection context");

var contextCmd_serverArg = new Argument<Uri>("server", "Server endpoint");

var contextCmd_usernameOpt = new Option<string>(name: "--username", description: "User login");
contextCmd_usernameOpt.AddAlias("-u");

var contextCmd_passwordOpt = new Option<string>(name: "--password", description: "User password");
contextCmd_passwordOpt.AddAlias("-p");

var contextCmd_tokenOpt = new Option<string>(name: "--token", description: "SA JWT token");
contextCmd_tokenOpt.AddAlias("-t");

var contextCmd_insecureOpt = new Option<bool>(name: "--insecure", description: "Skip verify SSL certificate", getDefaultValue: () => false);

contextCmd.Add(contextCmd_serverArg);
contextCmd.Add(contextCmd_usernameOpt);
contextCmd.Add(contextCmd_passwordOpt);
contextCmd.Add(contextCmd_tokenOpt);
contextCmd.Add(contextCmd_insecureOpt);

contextCmd.SetHandler(
    (Uri server, string username, string password, string token, bool insecure, OutputFormat format) =>
    {
        ContextModel model = new() { server = server, insecure = insecure, token = token };
        string serilizeContext = JsonSerializer.Serialize(model);
        File.WriteAllText("context.json", serilizeContext);
    }, contextCmd_serverArg, contextCmd_usernameOpt, contextCmd_passwordOpt, contextCmd_tokenOpt, contextCmd_insecureOpt, cmd_outFormatOpt);
#endregion


//var deleteCmd = new Command("delete", "Delete manifest");

#region status
var statusCmd = new Command("status", "Check status connect");
#endregion



cmd.AddGlobalOption(cmd_outFormatOpt);
cmd.Add(applyCmd);
cmd.Add(contextCmd);
cmd.Add(statusCmd);

return cmd.Invoke(args);