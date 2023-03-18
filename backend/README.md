# Backend

ASP.NET Core Web API project.

## References

- https://learn.microsoft.com/sr-cyrl-rs/visualstudio/javascript/tutorial-asp-net-core-with-angular?view=vs-2022
- https://codeburst.io/deploy-a-containerized-asp-net-core-app-to-heroku-using-github-actions-9e54c72db943
- https://faun.pub/vs-code-workflow-for-dockerize-asp-net-core-angular-app-c20e427c4a2
- https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-7.0&tabs=visual-studio
- https://blog.christian-schou.dk/how-to-build-a-restful-web-api-using-net-core6/
- https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=vs
- https://stackoverflow.com/questions/46147839/docker-how-can-i-have-sqlite-db-changes-persist-to-the-db-file
- https://code-maze.com/deep-dive-validators-fluentvalidation/
- https://blog.christian-schou.dk/how-to-use-fluentvalidation-in-asp-net-core6/
- https://kevsoft.net/2022/01/02/extra-validation-errors-in-asp-net-core.html
- https://code-maze.com/aspnetcore-modelstate-validation-web-api/
- https://code-maze.com/automapper-net-core/
- https://dev.to/cloudx/entity-framework-core-simplify-your-queries-with-automapper-3m8k
- https://andrewlock.net/handling-web-api-exceptions-with-problemdetails-middleware/
- https://codeopinion.com/problem-details-for-better-rest-http-api-errors/
- https://learn.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-7.0&tabs=visual-studio
- https://medium.com/geekculture/swagger-with-bearer-token-net6-b4ca5a8274b1
- https://codepedia.info/aspnet-core-jwt-refresh-token-authentication
- https://devblogs.microsoft.com/dotnet/jwt-validation-and-authorization-in-asp-net-core/
- https://codewithmukesh.com/blog/permission-based-authorization-in-aspnet-core/
- https://learn.microsoft.com/en-us/aspnet/core/security/authorization/iauthorizationpolicyprovider?view=aspnetcore-7.0
- https://sandrino.dev/blog/aspnet-core-5-jwt-authorization
- https://medium.com/c-sharp-progarmming/configure-annotations-in-swagger-documentation-for-asp-net-core-api-8215596907c7
- https://medium.com/@niteshsinghal85/enhance-swagger-documentation-with-annotations-in-asp-net-core-d2981803e299
- https://learn.microsoft.com/en-us/ef/core/modeling/data-seeding
- https://medium.com/agilix/entity-framework-core-enums-ee0f8f4063f2
- https://code-maze.com/efcore-modifying-data/
- https://www.c-sharpcorner.com/article/eager-loading-lazy-loading-and-explicit-loading-in-entity-framework/

## Test

"Funcionario"."Matricula": "46249/7"

{
  "id": 1,
  "dataInicio": "2021-01-01",
  "dataFim": "2021-12-31",
  "ferias": [
    {
      "dataInicio": "2022-01-01",
      "dataFim": "2022-01-14"
    },
    {
      "dataInicio": "2022-01-15",
      "dataFim": "2022-01-23"
    },
    {
      "dataInicio": "2022-01-24",
      "dataFim": "2022-01-30"
    }
  ],
  "funcionarioId": 1
}

| Perfil do Usuário | Credenciais do Usuário | Permissões do Usuário |
| ------------- | ------------------------------ | ----------- |
| Administrador | `{"cpf": "000.000.000-01","email": "rosangela@gesfer.com","senha": "Abc-1234"}` | - [x] BuscarFuncionario - [x] ListarFuncionarios  |
| Cadastrante | `{"cpf": "000.000.000-03","email": "joao@example.com","senha": "Abcd-1234"}` | ``` dada``` |
| Consultor |`{"cpf": "000.000.000-02","email": "maria@gesfer.com","senha": "123-Abcd"}`| ``` dada``` |
