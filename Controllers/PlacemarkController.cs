using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class PlacemarkController : ControllerBase
{
  private readonly KmlService _kmlService;

  public PlacemarkController()
  {
    _kmlService = new KmlService();
  }

  [HttpPost("export")]
  public IActionResult ExportPlacemarkData([FromBody] FilterRequest filters)
  {
    try
    {
      var placemarks = _kmlService.GetPlacemarkData();
      var filteredData = _kmlService.FilterPlacemarkData(placemarks, filters);

      var newKmlPath = "FilteredPlacemarkData.kml";

      return Ok(new { message = "KML exportado com sucesso!", filePath = newKmlPath });
    }
    catch (Exception ex)
    {
      return BadRequest(new { message = ex.Message });
    }
  }


  [HttpGet]
  public IActionResult GetPlacemarkData([FromQuery] FilterRequest filters)
  {
    try
    {
      var placemarks = _kmlService.GetPlacemarkData();
      var filteredData = _kmlService.FilterPlacemarkData(placemarks, filters);
      return Ok(filteredData);
    }
    catch (Exception ex)
    {
      return BadRequest(new { message = ex.Message });
    }
  }

  [HttpGet("filters")]
  public IActionResult GetAvailableFilters()
  {
    try
    {
      var placemarks = _kmlService.GetPlacemarkData();
      var filters = _kmlService.GetAvailableFilters(placemarks);
      return Ok(filters);
    }
    catch (Exception ex)
    {
      return BadRequest(new { message = ex.Message });
    }
  }
}
