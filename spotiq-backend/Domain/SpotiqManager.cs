using Microsoft.EntityFrameworkCore;
using spotiq_backend.DataAccess;
using spotiq_backend.Domain.Entities;

namespace spotiq_backend.Domain
{
    public class SpotiqManager
    {
        private readonly SpotiqContext _spotiqContext;
        public SpotiqManager(SpotiqContext spotiqContext)
        {
            _spotiqContext = spotiqContext;
        }

        
    }
}
