Feature: Test GraphQL Endpoint with Karate

Background: Base URL and Schemas
  * url baseUrl

  * text userSchema =
    """
      {
        id: '#string',
        name: '#string',
        email: '#string',
        age: '#number',
        birthday: '#string',
        address: '##addressSchema',
        friends: '##[] #userSchema'
      }
    """

  * text addressSchema =
    """
      {
        street: '#string',
        city: '#string',
        state: '#string',
        zip: '#string',
        url: '##string'
      }
    """

  * text articleSchema =
    """
      {
        id: '#string',
        title: '#string',
        content: '#string'
      }
    """

  * text nodeSchema =
    """
      {
        id: '#string'
      }
    """

Scenario: Perform a user query and validate the response
  * text query =
    """
      query UserTest($id: ID!) {
        user(id: $id) {
          id
          name
          email
          age
          birthday
          address {
            street
            city
            state
            zip
            url
          }
        }
      }
    """

  * text variables =
    """
      {
        "id": "68c06d52fec04d0d8a7ac0ace72875c0"
      }
    """

  Given path "/graphql"
  And request { query: query, operationName: "UserTest", variables: variables }
  When method post
  Then status 200
  And match response.data.user == userSchema

Scenario: Perform a users query and validate the response
  * text query =
    """
      query UsersTest($filter: UserFilterInput) {
        users(filter: $filter) {
          id
          name
          email
          age
          birthday
          address {
            street
            city
            state
            zip
            url
          }
        }
      }
    """

  * text variables =
    """
      {
        "filter": { "nameContains": "637ccb8e8b0f4c29a973876d80cec54b", "emailContains": "da434522a30141b3a026297c67b787d6", "ageGreaterThan": 231, "ageLessThan": 905, "address": { "streetContains": "ad6dc0b6cf6940c680adf2de3f94035a", "cityContains": "8b4555c2c0c84b148531a3bcbbf45457", "stateEquals": "6844abe5dfc542b58d661854252b88e7", "zipEquals": "e4726271bd5544ca9e91295f301c018a", "url": <some value> } }
      }
    """

  Given path "/graphql"
  And request { query: query, operationName: "UsersTest", variables: variables }
  When method post
  Then status 200
  And match each response.data.users == userSchema

Scenario: Perform a search query and validate the response
  * text query =
    """
      query SearchTest($query: String!, $limit: Int, $offset: Int) {
        search(query: $query) {
          ... on User {
            id
            name
            email
            age
            birthday
            address {
              street
              city
              state
              zip
              url
            }
            friends(limit: $limit, offset: $offset) {
              id
              name
              email
              age
              birthday
              address {
                street
                city
                state
                zip
                url
              }
            }
          }
          ... on Article {
            id
            title
            content
          }
        }
      }
    """

  * text variables =
    """
      {
        "query": "35fe1ec4d7034a6d91a09baa5f32e961",
        "limit": 715,
        "offset": 649
      }
    """

  * def isValid =
    """
    response =>
      karate.match(response, userSchema).pass ||
      karate.match(response, articleSchema).pass
    """

  Given path "/graphql"
  And request { query: query, operationName: "SearchTest", variables: variables }
  When method post
  Then status 200
  And match each response.data.search == "#? isValid(_)"

Scenario: Perform a article query and validate the response
  * text query =
    """
      query ArticleTest($id: ID!) {
        article(id: $id) {
          id
          title
          content
        }
      }
    """

  * text variables =
    """
      {
        "id": "b9f3dd7defcc4d81a548a152567d7ded"
      }
    """

  Given path "/graphql"
  And request { query: query, operationName: "ArticleTest", variables: variables }
  When method post
  Then status 200
  And match response.data.article == articleSchema

Scenario: Perform a articles query and validate the response
  * text query =
    """
      query ArticlesTest($input: ArticleInput) {
        articles(input: $input) {
          id
          title
          content
        }
      }
    """

  * text variables =
    """
      {
        "input": { "titleContains": "ff281cc50c5e40f4bc212fbefad3f26c", "contentContains": "3560d172b0434622b761f89a5c95ae56", "authorId": "2fc98ef9a8374923be58c8e5187d8468" }
      }
    """

  Given path "/graphql"
  And request { query: query, operationName: "ArticlesTest", variables: variables }
  When method post
  Then status 200
  And match each response.data.articles == articleSchema