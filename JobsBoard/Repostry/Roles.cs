using JobsBoard.Areas.Identity.Data;
using JobsBoard.Data;
using JobsBoard.Repostry.RepostryPattern;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JobsBoard.Repostry
{
    public class Roles : RepostryPattern<IdentityRole>
    {
        private readonly JobsBoardContext _Context;
        private readonly ILogger<Roles> _logger;

        private readonly RoleManager<IdentityRole> _roleManager;

        public Roles(JobsBoardContext context, ILogger<Roles> logger, RoleManager<IdentityRole> roleManager)
        {
            _Context = context;
            _logger = logger;
            _roleManager = roleManager;

        }

        public async Task Create(IdentityRole entity)
        {

            if (string.IsNullOrEmpty(entity.NormalizedName))
            {
                entity.NormalizedName = entity.Name.ToUpper(); // Normalize the name
            }

            try
            {

                _Context.Roles.Add(entity);
                await SaveChange();
                //if (!await _roleManager.RoleExistsAsync(entity.Name))
                //{
                //    await _roleManager.CreateAsync(new IdentityRole(entity.Name));
                //}

                //if (!result.Succeeded)
                //{
                //    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                //    _logger.LogError($"Failed to create role: {errors}");
                //    throw new InvalidOperationException($"Failed to create role: {errors}");
                //}
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while creating the role: {ex.Message}");
                throw;  // Rethrow exception
            }

        }

        public async Task Delete<TId>(TId id)
        {
            try
            {
                var stringId = id.ToString();
                var role = await _roleManager.FindByIdAsync(stringId);

                if (role != null)
                {
                    var result = await _roleManager.DeleteAsync(role);

                    if (!result.Succeeded)
                    {
                        // Log any errors if the deletion fails
                        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                        _logger.LogError($"Failed to delete role: {errors}");
                        throw new InvalidOperationException($"Failed to delete role: {errors}");
                    }
                }
                else
                {
                    _logger.LogWarning($"Role with ID {stringId} not found.");
                }
            }
            catch (Exception ex)
            {
                // Log the exception if something goes wrong
                _logger.LogError($"An error occurred while deleting the role: {ex.Message}");
                throw; // Rethrow exception
            }
        }


        public async Task<IdentityRole> Find<TId>(TId id)
        {
            var stringId = id.ToString();
            return await _Context.Roles.FirstOrDefaultAsync(x => x.Id == stringId); 
        }

        public async Task<IList<IdentityRole>> List()
        {
            return await _Context.Roles.ToListAsync(); 
        }

        public async Task<List<IdentityRole>> Search(string term)
        {
            return await _Context.Roles
                .Where(x => x.Name.Contains(term)) 
                .ToListAsync(); 
        }

        public async Task Update<TId>(TId id, IdentityRole Role)
        {
         
                _Context.Update(Role);
                await SaveChange();
            
        }
        private async Task SaveChange()
        {
            await _Context.SaveChangesAsync();
        }

    }
}
