using Aspire.C4;

var softwareSystemContextBuilder = SoftwareSystemContext.CreateBuilder<CommerceFlowWebApi>();

var app = softwareSystemContextBuilder.Builder.Build();

await app.RunAsync();