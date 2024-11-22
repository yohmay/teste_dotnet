using Microsoft.OpenApi.Models;
using System.Xml.Linq;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
  c.SwaggerDoc("v1", new OpenApiInfo { Title = "KML API", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI(c =>
  {
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "KML API v1");
    c.RoutePrefix = string.Empty;
  });
}

app.UseHttpsRedirection();

var kmlFilePath = "DIRECIONADORES1.kml";
var kmlElements = LoadKmlData(kmlFilePath);

app.MapGet("/api/placemarks", (string? cliente, string? situacao, string? bairro) =>
{
  var filteredData = FilterData(kmlElements, cliente, situacao, bairro);
  return Results.Ok(filteredData);
});


app.MapPost("/api/placemarks/export", (string cliente, string situacao, string bairro) =>
{
  var filteredData = FilterData(kmlElements, cliente, situacao, bairro);
  var newKml = ExportKml(filteredData);
  return Results.Ok(new { message = "KML exportado com sucesso", filePath = newKml });
});

app.MapGet("/api/placemarks/filters", () =>
{
  var filters = GetUniqueFilters(kmlElements);
  return Results.Ok(filters);
});

app.Run();

List<dynamic> LoadKmlData(string filePath)
{
  var elements = new List<dynamic>();

  var kmlDoc = XDocument.Load(filePath);
  var placemarks = kmlDoc.Descendants("Placemark");

  foreach (var placemark in placemarks)
  {
    elements.Add(new
    {
      Cliente = placemark.Element("Cliente")?.Value,
      Situacao = placemark.Element("Situacao")?.Value,
      Bairro = placemark.Element("Bairro")?.Value,
    });
  }

  return elements;
}

List<dynamic> FilterData(List<dynamic> data, string? cliente, string? situacao, string? bairro)
{
  return data.Where(d =>
      (string.IsNullOrEmpty(cliente) || d.Cliente.Contains(cliente, StringComparison.OrdinalIgnoreCase)) &&
      (string.IsNullOrEmpty(situacao) || d.Situacao.Contains(situacao, StringComparison.OrdinalIgnoreCase)) &&
      (string.IsNullOrEmpty(bairro) || d.Bairro.Contains(bairro, StringComparison.OrdinalIgnoreCase))
  ).ToList();
}

string ExportKml(List<dynamic> filteredData)
{
  var kmlDoc = new XDocument(new XElement("kml", new XElement("Document")));
  var kmlElement = kmlDoc.Element("kml").Element("Document");

  foreach (var placemark in filteredData)
  {
    kmlElement.Add(new XElement("Placemark",
        new XElement("Cliente", placemark.Cliente),
        new XElement("Situacao", placemark.Situacao),
        new XElement("Bairro", placemark.Bairro)
    ));
  }

  var filePath = "FilteredPlacemarkData.kml";
  kmlDoc.Save(filePath);
  return filePath;
}

object GetUniqueFilters(List<dynamic> kmlElements)
{
  return new
  {
    Clientes = kmlElements.Select(e => e.Cliente).Distinct().ToList(),
    Situacoes = kmlElements.Select(e => e.Situacao).Distinct().ToList(),
    Bairros = kmlElements.Select(e => e.Bairro).Distinct().ToList()
  };
}
