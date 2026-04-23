@updateUsuario
Feature: Atualizar usuario

  Scenario: Atualizar usuario com dados validos
    Given que existe um usuario com id "1"
    And que informei para atualizacao o nome "Pedro Atualizado"
    And que informei para atualizacao o email "pedro.atualizado@email.com"
    And que informei para atualizacao o status ativo como true
    When eu solicitar a atualizacao do usuario com id "1"
    Then a atualizacao do usuario deve ser realizada com sucesso

  Scenario: Nao atualizar usuario quando ele nao existir
    Given que nao existe um usuario com id "99"
    And que informei para atualizacao o nome "Pedro Atualizado"
    And que informei para atualizacao o email "pedro.atualizado@email.com"
    And que informei para atualizacao o status ativo como true
    When eu solicitar a atualizacao do usuario com id "99"
    Then o sistema deve informar que nao foi possivel atualizar o usuario