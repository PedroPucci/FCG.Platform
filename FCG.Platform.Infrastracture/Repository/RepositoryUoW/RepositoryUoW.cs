using FCG.Platform.Domain.Interfaces.Repositories;
using FCG.Platform.Infrastracture.Connections;
using Microsoft.EntityFrameworkCore.Storage;
using Serilog;

namespace FCG.Platform.Infrastracture.Repository.RepositoryUoW
{
    public class RepositoryUoW : IRepositoryUoW
    {
        private readonly DataContext _context;
        private bool _disposed = false;
        private IUserRepository? _userEntityRepository = null;
        private IGameRepository? _gameEntityRepository = null;

        public RepositoryUoW(DataContext context)
        {
            _context = context;
        }

        public IUserRepository UserRepository
        {
            get
            {
                if (_userEntityRepository is null)
                {
                    _userEntityRepository = new UserRepository(_context);
                }
                return _userEntityRepository;
            }
        }

        public IGameRepository GameRepository
        {
            get
            {
                if (_gameEntityRepository is null)
                {
                    _gameEntityRepository = new GameRepository(_context);
                }
                return _gameEntityRepository;
            }
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public IDbContextTransaction BeginTransaction()
        {
            return _context.Database.BeginTransaction();
        }

        public void Commit()
        {
            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Log.Error($"Database connection failed: {ex.Message}");
                throw new ApplicationException("Database is not available. Please check the connection.");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _context.Dispose();
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}