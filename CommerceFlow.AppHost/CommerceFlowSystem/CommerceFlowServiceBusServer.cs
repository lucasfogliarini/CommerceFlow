using Aspire.C4;
public class CommerceFlowServiceBusServer : Service
{
    public override string Name => "ServiceBusServer";

    public override void Configure(SoftwareSystemContextBuilder system)
    {
        var serviceBusResourceBuilder = system.Builder.AddAzureServiceBus(Name)
                                        .RunAsEmulator(emulator =>
                                        {
                                            emulator.WithLifetime(ContainerLifetime.Persistent);
                                            emulator.WithHostPort(5672);
                                        });

        var asbUi = system.Builder.AddAsbEmulatorUi("asb-ui", serviceBusResourceBuilder, system.GetNextPort());
        system.AddResourceBuilder(asbUi);        

        system.AddResourceBuilder(serviceBusResourceBuilder);
    }
}