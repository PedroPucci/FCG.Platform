using FluentAssertions;
using Reqnroll;
using FCG.Platform.Domain.Entities.Entity;
using FCG.Platform.Domain.Entities.Dto.UserDto;

namespace FCG.Plataform.Bdd.Steps
{
    [Binding]
    public class AtualizarUsuarioSteps
    {
        private readonly Dictionary<string, UserEntity> _usuarios = new();
        private readonly UpdateUserRequest _request = new();

        private bool _resultado;

        [Given(@"que existe um usuario com id ""(.*)""")]
        public void GivenQueExisteUmUsuarioComId(string id)
        {
            _usuarios[id] = new UserEntity
            {
                Id = id,
                Name = "Nome Antigo",
                Email = "email.antigo@email.com",
                IsActive = false
            };
        }

        [Given(@"que nao existe um usuario com id ""(.*)""")]
        public void GivenQueNaoExisteUmUsuarioComId(string id)
        {
            if (_usuarios.ContainsKey(id))
                _usuarios.Remove(id);
        }

        [Given(@"que informei para atualizacao o nome ""(.*)""")]
        public void GivenQueInformeiParaAtualizacaoONome(string nome)
        {
            _request.Name = nome;
        }

        [Given(@"que informei para atualizacao o email ""(.*)""")]
        public void GivenQueInformeiParaAtualizacaoOEmail(string email)
        {
            _request.Email = email;
        }

        [Given(@"que informei para atualizacao o status ativo como true")]
        public void GivenStatusAtivoTrue()
        {
            _request.IsActive = true;
        }

        [Given(@"que informei para atualizacao o status ativo como false")]
        public void GivenStatusAtivoFalse()
        {
            _request.IsActive = false;
        }

        [When(@"eu solicitar a atualizacao do usuario com id ""(.*)""")]
        public void WhenEuSolicitarAAtualizacaoDoUsuarioComId(string id)
        {
            if (!_usuarios.TryGetValue(id, out var user))
            {
                _resultado = false;
                return;
            }

            user.Name = _request.Name;
            user.Email = _request.Email;
            user.IsActive = _request.IsActive;
            user.ModificationDate = DateTime.UtcNow;

            _resultado = true;
        }

        [Then(@"a atualizacao do usuario deve ser realizada com sucesso")]
        public void ThenAAtualizacaoDoUsuarioDeveSerRealizadaComSucesso()
        {
            _resultado.Should().BeTrue();
        }

        [Then(@"o sistema deve informar que nao foi possivel atualizar o usuario")]
        public void ThenOSistemaDeveInformarQueNaoFoiPossivelAtualizarOUsuario()
        {
            _resultado.Should().BeFalse();
        }
    }
}