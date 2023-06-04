using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Primitives;
using MillionThings.SimpleWebApp.Pages;
using Moq;

namespace MillionThings.Tests.SimpleWebApp;

public class IndexPagesTest
{
    [Fact]
    public void ShouldAddATaskWhenPostingOne()
    {
        var sut = new IndexModel(NullLogger<IndexModel>.Instance, Guid.NewGuid() + ".json");

        sut.PageContext = CreateContext(new() { { "todo", sut.CurrentTodoId } });

        sut.OnPost("this is a test");
        Assert.Single(sut.Tasks);
    }

    private PageContext CreateContext(Dictionary<string, StringValues> queryParams)
    {
        var httpContextMock = new Mock<HttpContext>();
        var httpRequestMock = new Mock<HttpRequest>();

        // Set up the QueryString and Query properties
        httpRequestMock.Setup(r => r.QueryString)
            .Returns(new QueryString("?" + string.Join("&", queryParams.Select(kv => kv.Key + "=" + kv.Value))));
        httpRequestMock.Setup(r => r.Query)
            .Returns(new QueryCollection(queryParams));

        // Assign the mocked HttpRequest to the mocked HttpContext
        httpContextMock.Setup(c => c.Request).Returns(httpRequestMock.Object);

        return new PageContext
        {
            HttpContext = httpContextMock.Object
        };
    }
}