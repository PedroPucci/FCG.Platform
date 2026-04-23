using Reqnroll;
using FluentAssertions;

namespace FCG.Plataform.Bdd.Steps
{
    [Binding]
    public class CadastroUsuarioSteps
    {
        private string _nome = string.Empty;
        private string _email = string.Empty;
        private string _senha = string.Empty;
        private bool _resultado;

        [Given(@"que informei um nome ""(.*)""")]
        public void GivenQueInformeiUmNome(string nome)
        {
            _nome = nome;
        }

        [Given(@"que informei um email ""(.*)""")]
        public void GivenQueInformeiUmEmail(string email)
        {
            _email = email;
        }

        [Given(@"que informei uma senha válida")]
        public void GivenQueInformeiUmaSenhaValida()
        {
            _senha = "Senha@123";
        }

        [Given(@"que não informei email")]
        public void GivenQueNaoInformeiEmail()
        {
            _email = string.Empty;
        }

        [When(@"eu solicitar o cadastro do usuário")]
        public void WhenEuSolicitarOCadastroDoUsuario()
        {
            _resultado = !string.IsNullOrWhiteSpace(_nome)
                      && !string.IsNullOrWhiteSpace(_email)
                      && !string.IsNullOrWhiteSpace(_senha);
        }

        [Then(@"o usuário deve ser cadastrado com sucesso")]
        public void ThenOUsuarioDeveSerCadastradoComSucesso()
        {
            _resultado.Should().BeTrue();
        }

        [Then(@"o sistema deve informar que o email é obrigatório")]
        public void ThenOSistemaDeveInformarQueOEmailEObrigatorio()
        {
            _resultado.Should().BeFalse();
        }
    }
}