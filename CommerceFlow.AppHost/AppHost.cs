using Aspire.C4;

var systemBuilder = SoftwareSystemContext.Configure<CommerceFlowSystem>();

var app = systemBuilder.Build();

await app.RunAsync();