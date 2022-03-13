// https://docs.microsoft.com/ru-ru/dotnet/standard/commandline/

using System.CommandLine;
using System.CommandLine.Invocation;
using GrantAccessCLI;

#region APPLY
var applyCmd = new Command("apply", "Применить манифест");

var applyCmd_FileArg = new Argument<FileInfo>("file", "Путь до файла с манифестом");

applyCmd.Add(applyCmd_FileArg);

applyCmd.SetHandler(
    (FileInfo file) =>
    {
        Console.WriteLine(file.FullName);
    }, applyCmd_FileArg);
#endregion

#region context
var contextCmd = new Command("context", "Установить значения контекста");

var contextCmd_serverArg = new Argument<Uri>("server", "Адрес сервера");

var contextCmd_usernameOpt = new Option<string>(name: "--username", description: "Логин пользователя");
contextCmd_usernameOpt.AddAlias("-u");

var contextCmd_passwordOpt = new Option<string>(name: "--password", description: "Пароль пользователя");
contextCmd_passwordOpt.AddAlias("-p");

var contextCmd_tokenOpt = new Option<string>(name: "--token", description: "Токен сервисного аккаунта");
contextCmd_tokenOpt.AddAlias("-t");

contextCmd.Add(contextCmd_serverArg);
contextCmd.Add(contextCmd_usernameOpt);
contextCmd.Add(contextCmd_passwordOpt);
contextCmd.Add(contextCmd_tokenOpt);

contextCmd.SetHandler(
    (Uri server, string username, string password, string token) =>
    {
        Console.WriteLine(server.Port);
    }, contextCmd_serverArg, contextCmd_usernameOpt, contextCmd_passwordOpt, contextCmd_tokenOpt);
#endregion


var deleteCmd = new Command("delete", "Удалить манифест");



var cmd = new RootCommand();


cmd.Add(applyCmd);
cmd.Add(contextCmd);

return cmd.Invoke(args);