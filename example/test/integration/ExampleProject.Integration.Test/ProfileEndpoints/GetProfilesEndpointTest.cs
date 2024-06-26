using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Xunit.Abstractions;

using ExampleProject.Integration.Test;

namespace ExampleProject.Integration.Test.ProfileEndpoints;

/// <summary>
/// Integration tests for creating a {name].
/// </summary>
public class GetProfilesEndpointTest : IClassFixture<ExampleProjectApiFactory>
{
    private const string BaseUrl = "api/{name|lower}s";
    private readonly HttpClient _client;
	
	/// <summary>
	/// Initializes a new instance of the <see cref="GetProfilesEndpointTest"/> class.
	/// </summary>
	/// <param name="apiFactory"></param>
	public GetProfilesEndpointTest(ExampleProjectApiFactory apiFactory)
	{
		_client = apiFactory.CreateClient();
	}
}