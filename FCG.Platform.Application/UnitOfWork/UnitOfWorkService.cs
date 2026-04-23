using FCG.Platform.Application.Services;
using FCG.Platform.Infrastracture.Repository.RepositoryUoW;

namespace FCG.Platform.Application.UnitOfWork
{
    public class UnitOfWorkService : IUnitOfWorkService
    {
        private readonly IRepositoryUoW _repositoryUoW;
        private UserService userService;
        private GameService gameService;
        private AuthenticationService authenticationService;

        public UnitOfWorkService(IRepositoryUoW repositoryUoW)
        {
            _repositoryUoW = repositoryUoW;
        }

        public UserService UserService
        {
            get
            {
                if (userService is null)
                    userService = new UserService(_repositoryUoW);
                return userService;
            }
        }

        public GameService GameService
        {
            get
            {
                if (gameService is null)
                    gameService = new GameService(_repositoryUoW);
                return gameService;
            }
        }

        public AuthenticationService AuthenticationService
        {
            get
            {
                if (authenticationService is null)
                    authenticationService = new AuthenticationService(_repositoryUoW);
                return authenticationService;
            }
        }
    }
}