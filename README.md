# 🎮 FIAP Cloud Games (FCG) - Game Service

## 📚 Sobre o Projeto

Fiap Cloud Games (FCG) é uma plataforma inovadora de jogos na nuvem desenvolvida dentro do ecossistema educacional da FIAP (Faculdade de Informática e Administração Paulista). O projeto tem como objetivo oferecer aos alunos uma experiência prática e integrada no desenvolvimento, deployment e consumo de jogos hospedados em ambientes cloud.

[Documentação](https://www.notion.so/Fiap-Cloud-Games-FCG-1dea50ade75480e78653c05e2cca2193?pvs=4)

## 🎮 Sobre o Serviço de Jogos

O serviço de jogos é responsável por gerenciar o catálogo de jogos e promoções disponíveis na plataforma FCG. Ele oferece funcionalidades para criar, ler, atualizar e deletar informações sobre os jogos e as promoções.

### <img align="center" height="30" width="40" src="https://raw.githubusercontent.com/devicons/devicon/master/icons/elasticsearch/elasticsearch-original.svg"> Elastic Search

Os dados dos jogos são armazenados no Elastic Cloud. A classe que cuida da conexão é <b>"FCG.Game.API/Configurations/ElasticSearchConfig.cs"</b> e a classe que faz as logicas de consultas é <b>"FCG.Game.Application/Services/GameServices.cs"</b>. Enquanto os dados de promoções são salvos no SQLServer.

### :computer: Comunicação com a api de usuarios

Para usar a API de Games é necessário <b>fazer autenticação via token JWT obtido pelo metodo de login da api de Usuários</b>, após isso no arquivo <b>"FCG.Game.API/Controllers/ApiBaseController.cs"</b> extraimos o Id e o Token do usuário. Esses dados são usados para logar-se na Api de usuarios depois para então recuperar os itens da biblioteca de jogos do usuário.

A partir desses dados nós pesquisamos no <b>Elastic Cloud</b> quais são os generos desses jogos que o usuário possui, então pegamos o genero mais frequente e retornamos para o usuario outros jogos do mesmo genero no metodo de recomendações. 

Caso sua biblioteca seja vazia retornamos a lista de jogos mais populares.
      
## ⚙️ Tecnologias e Plataformas utilizadas

- [.NET 8](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio](https://visualstudio.microsoft.com/pt-br/)
- [EF Core](https://learn.microsoft.com/pt-br/ef/core/)
- [ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/)
- [XUnit](https://xunit.net/)
- [Swagger](https://swagger.io/)
- [Docker](https://www.docker.com/)
- Elastic Cloud | Elastic Search
  
## 🛠️ Como Executar

### Usando Docker

1. Certifique-se de ter o [Docker](https://www.docker.com/get-started/) instalado em sua máquina.
2. No terminal, navegue até a raiz do projeto.
3. Execute o comando abaixo para construir e iniciar os containers:

```bash
docker-compose up -d --build
```

4. O serviço estará disponível em `http://localhost:5002/`.


## 🧪 Testes

- Para rodar os testes, utilize o **Test Explorer** do Visual Studio.
- Ou execute via terminal:

```bash
dotnet test
```

## 🤝 Contribuição

Contribuições são bem-vindas! Sinta-se à vontade para abrir issues ou pull requests.

## 📄 Licença

Este projeto está licenciado sob a licença MIT.

---

Feito com ❤️!
