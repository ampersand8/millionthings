using Microsoft.Extensions.Logging.Abstractions;
using MillionThings.SimpleWebApp.Pages;

namespace MillionThings.Tests.SimpleWebApp;

public class IndexPagesTest
{
    [Fact]
    public void ShouldAddATaskWhenPostingOne()
    {
        var sut = new IndexModel(NullLogger<IndexModel>.Instance, Guid.NewGuid() + ".json");
        sut.OnPost("this is a test");
        Assert.Single(sut.Todos);
    }
}