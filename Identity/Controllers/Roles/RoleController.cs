using Identity.Models;
using Identity.Repositories.Roles;
using Identity.Services.UnitOfWorks;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers.Roles;

[ApiController]
[Route("api/roles")]
public class RoleController(IUnitOfWork unitOfWork, IRoleRepository roleRepository) : ControllerBase
{
    private readonly IRoleRepository _roleRepository = roleRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var roles = await _roleRepository.GetAllAsync();
        return Ok(roles);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var isExist = await _roleRepository.IsExist(id);

        if (!isExist)
        {
            return NotFound();
        }

        var role = await _roleRepository.GetAsync(id);
        return Ok(role);

    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddRequest request)
    {
        var isExist = await _roleRepository.IsExist(request.Name);

        if (isExist)
        {
            return BadRequest("Role already exist");
        }

        var role = new Role { Name = request.Name };

        await _roleRepository.AddAsync(role);
        await _unitOfWork.Commit();

        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var isExist = await _roleRepository.IsExist(id);

        if (!isExist)
        {
            return NotFound();
        }

        await _roleRepository.DeleteAsync(id);
        await _unitOfWork.Commit();

        return Ok();
    }

}
