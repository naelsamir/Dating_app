using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BuggyController : ControllerBase
{
        private readonly DataContext _dataContext;
    public BuggyController(DataContext dataContext)
    {
            this._dataContext = dataContext;
    }

    [HttpGet("auth")] 
    public ActionResult<string> GetSecret(){
        return Unauthorized();
    }

    [HttpGet("not-found")] 
    public ActionResult<AppUser> getNotFound(){
        var thing= _dataContext.Users.Find(-1);
        if(thing ==null) return NotFound();
        return thing;   
    }

    [HttpGet("server-error")] 
    public ActionResult<string> getServerError(){
        var thing= _dataContext.Users.Find(-1);
        var thingToReturn = thing.ToString();
        return thingToReturn;   
    }

    [HttpGet("bad-request")] 
    public ActionResult<string> getBadRequest(){
        return BadRequest("this is not a good request");   
    }

}
