namespace Architecture.Tests;

public abstract class BaseTest
{
    protected static readonly Assembly DomainAssembly = typeof(DomainAssembly).Assembly;
    protected static readonly Assembly ApplicationAssembly = typeof(InfrastructureAssembly).Assembly;
    protected static readonly Assembly InfrastructureAssembly = typeof(ApplicationAssembly).Assembly;
    protected static readonly Assembly PresentationAssembly = typeof(PresentationAssembly).Assembly;
}