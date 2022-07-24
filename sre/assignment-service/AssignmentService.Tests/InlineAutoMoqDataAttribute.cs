namespace AssignmentService.Tests;

public class InlineAutoMoqDataAttribute : InlineAutoDataAttribute
{
    public InlineAutoMoqDataAttribute(params object[] objects) : base(new AutoDomainDataAttribute(), objects) { }
}
