@cadastro
Feature: Cadastro de usuário

  Scenario: Cadastrar usuário com dados válidos
    Given que informei um nome "Pedro"
    And que informei um email "pedro@email.com"
    And que informei uma senha válida
    When eu solicitar o cadastro do usuário
    Then o usuário deve ser cadastrado com sucesso

  Scenario: Não cadastrar usuário sem email
    Given que informei um nome "Pedro"
    And que não informei email
    And que informei uma senha válida
    When eu solicitar o cadastro do usuário
    Then o sistema deve informar que o email é obrigatório