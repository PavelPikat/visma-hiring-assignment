using System.Net;
using System.Net.Http.Headers;
using AssignmentService.Server;
using AssignmentService.Server.Exceptions;
using FluentAssertions;
using Moq.Contrib.HttpClient;

namespace AssignmentService.Tests;

public class PdfServiceTests
{
    private const string HttpClientBaseUrl = "http://localhost:3000/";

    private string CorruptedPdfPath =>
        Path.Combine(
            Path.GetFullPath(Path.GetDirectoryName(GetType().Assembly.Location)!), "TestFiles",
            "corrupt-dummy.pdf");

    [Theory]
    [InlineAutoMoqData("application/png", "dummy.png")]
    [InlineAutoMoqData("application/pdf", "dummy.pdf")]
    public async Task GetPdfOrPngStream_Should_Return_Correct_ContentType(string contentType, string testFileName, [Frozen] Mock<HttpMessageHandler> messageHandler,
        [Frozen] Mock<IHttpClientFactory> httpClientFactory, PdfService sut)
    {
        // Arrange
        var testFilePath =  Path.Combine(
            Path.GetFullPath(Path.GetDirectoryName(GetType().Assembly.Location)!), "TestFiles",
            testFileName);
        
        messageHandler.SetupRequest(HttpMethod.Get, HttpClientBaseUrl)
            .ReturnsResponse(HttpStatusCode.OK,
                new StreamContent(File.OpenRead(testFilePath)),
                configure: response =>
                {
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                });

        httpClientFactory.Setup(
                x => x.CreateClient(ServiceConfigOptions.PdfClientName))
            .Returns(() =>
            {
                var client = messageHandler.CreateClient();
                client.BaseAddress = new Uri(HttpClientBaseUrl);
                return client;
            });

        // Act
        var result = await sut.GetPdfOrPngStream(int.MaxValue);

        // Assert
        result.ContentType.Should().Be(contentType, "content type should match");
    }

    [Theory, AutoDomainData]
    public async Task GetPdfOrPngStream_Should_Throw_On_Corrupted_Pdf([Frozen] Mock<HttpMessageHandler> messageHandler,
        [Frozen] Mock<IHttpClientFactory> httpClientFactory, PdfService sut)
    {
        // Arrange
        messageHandler.SetupRequest(HttpMethod.Get, HttpClientBaseUrl)
            .ReturnsResponse(HttpStatusCode.OK,
                new StreamContent(File.OpenRead(CorruptedPdfPath)),
                configure: response =>
                {
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                });

        httpClientFactory.Setup(
                x => x.CreateClient(ServiceConfigOptions.PdfClientName))
            .Returns(() =>
            {
                var client = messageHandler.CreateClient();
                client.BaseAddress = new Uri(HttpClientBaseUrl);
                return client;
            });

        // Act
        Func<Task> act = () => sut.GetPdfOrPngStream(int.MaxValue);

        // Assert
        await act.Should().ThrowAsync<CorruptedResponseException>("PDF is corrupted");
    }
    
    [Theory, AutoDomainData]
    public async Task GetPdfOrPngStream_Should_Throw_On_Empty_ContentType([Frozen] Mock<HttpMessageHandler> messageHandler,
    [Frozen] Mock<IHttpClientFactory> httpClientFactory, PdfService sut)
    {
        // Arrange
        messageHandler.SetupRequest(HttpMethod.Get, HttpClientBaseUrl)
            .ReturnsResponse(HttpStatusCode.OK);

        httpClientFactory.Setup(
                x => x.CreateClient(ServiceConfigOptions.PdfClientName))
            .Returns(() =>
            {
                var client = messageHandler.CreateClient();
                client.BaseAddress = new Uri(HttpClientBaseUrl);
                return client;
            });

        // Act
        Func<Task> act = () => sut.GetPdfOrPngStream(int.MaxValue);

        // Assert
        await act.Should().ThrowAsync<InvalidContentTypeException>("ContentType header is invalid");
    }
}
