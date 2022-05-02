# Reacher

Use the [Strike API](https://developer.strike.me/) to get paid to receive emails.

Find the website running at https://www.reacher.me.

<img src="https://user-images.githubusercontent.com/95240139/166231741-6c2ccfcf-c042-45ce-8ad2-ef32950caa75.png" width="500px"/>

  * [Development](#development)
  * [Configuration](#configuration)
  * [Contribution](#contribution)

## Development

The solution was built using Visual Studio 2022. Open the `reacher.sln` solution and run the `Reacher.app` project to launch the website.

## Configuration

This solution uses [.NET user secrets](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0&tabs=windows) to secure the secrets for the application.

You will need to set these secrets to run the `Reacher.App` and `Reacher.Tests` projects.

```json
{
  "ConnectionStrings": {
    "AppDb": "[SQL Server Connection String]",
    "AzureStorage": "[Azure Storage Connection String]"
  },
  "Oidc": {
    "Issuer": "[https://auth.next.strike.me/]",
    "ClientId": "[Strike Client App ID]",
    "ClientSecret": "[Strike Client App Secret]"
  },
  "ReacherSettings": {
    "StrikeApiUrl": "[Strike API URL]",
    "StrikeApiKey": "[Strike API Key]",
    "SendGridApiKey": "[SendGrid API Key]"
  }
}
```

## Contribution

Good ways to contribute are to try things out, file issues, join in design conversations, and make pull-requests.

If you would like to contribute in a more meaningful way, please contact us at dev@reacher.me
