using System.Xml.Linq;

public class KmlService
{
  private string _kmlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "DIRECIONADORES1.kml");


  public List<Placemark> GetPlacemarkData()
  {
    var doc = XDocument.Load(_kmlFilePath);
    var placemarks = doc.Descendants("Placemark")
                        .Select(p => new Placemark
                        {
                          Cliente = p.Element("Cliente")?.Value ?? string.Empty,
                          Situacao = p.Element("Situacao")?.Value ?? string.Empty,
                          Bairro = p.Element("Bairro")?.Value ?? string.Empty,
                        })
                        .ToList();
    Console.WriteLine($"Total de placemarks encontrados: {placemarks.Count}");
    foreach (var p in placemarks)
    {
      Console.WriteLine($"Cliente: {p.Cliente}, Situação: {p.Situacao}, Bairro: {p.Bairro}");
    }


    return placemarks;
  }

  public List<Placemark> FilterPlacemarkData(List<Placemark> placemarks, FilterRequest filters)
  {

    if (!string.IsNullOrEmpty(filters.Cliente))
    {
      placemarks = placemarks.Where(p => p.Cliente.Contains(filters.Cliente, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    if (!string.IsNullOrEmpty(filters.Situacao))
    {
      placemarks = placemarks.Where(p => p.Situacao.Contains(filters.Situacao, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    if (!string.IsNullOrEmpty(filters.Bairro))
    {
      placemarks = placemarks.Where(p => p.Bairro.Contains(filters.Bairro, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    return placemarks;
  }

  public List<string> GetAvailableFilters(List<Placemark> placemarks)
  {
    var clientes = placemarks.Select(p => p.Cliente).Distinct().ToList();
    var situacoes = placemarks.Select(p => p.Situacao).Distinct().ToList();
    var bairros = placemarks.Select(p => p.Bairro).Distinct().ToList();

    return clientes.Concat(situacoes).Concat(bairros).Distinct().ToList();
  }
}
