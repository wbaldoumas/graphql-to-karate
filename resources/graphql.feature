Feature: Test GraphQL Endpoint with Karate

Background: Base URL and Schemas
  * url 'https://countries.trevorblades.com'

  * def continentSchema =
    """
      {
        code: '##string',
        name: '##string',
        countries: '##[] ##(countrySchema)'
      }
    """

  * def countrySchema =
    """
      {
        code: '##string',
        name: '##string',
        native: '##string',
        phone: '##string',
        currency: '##string',
        languages: '##[] ##(languageSchema)',
        emoji: '##string',
        emojiU: '##string',
        states: '##[] ##(stateSchema)'
      }
    """

  * def languageSchema =
    """
      {
        code: '##string',
        name: '##string',
        native: '##string',
        rtl: '##boolean'
      }
    """

  * def stateSchema =
    """
      {
        code: '##string',
        name: '##string'
      }
    """

Scenario: Perform a continents query and validate the response
  * text query =
    """
      query ContinentsTest {
        continents {
          code
          name
          countries {
            code
            name
            native
            phone
            currency
            languages {
              code
              name
              native
              rtl
            }
            emoji
            emojiU
            states {
              code
              name
            }
          }
        }
      }
    """

  Given path "/graphql"
  And request { query: '#(query)', operationName: "ContinentsTest" }
  When method post
  Then status 200
  And match each response.data.continents == "##(continentSchema)"

Scenario: Perform a continent query and validate the response
  * text query =
    """
      query ContinentTest($code: ID!) {
        continent(code: $code) {
          code
          name
          countries {
            code
            name
            native
            phone
            currency
            languages {
              code
              name
              native
              rtl
            }
            emoji
            emojiU
            states {
              code
              name
            }
          }
        }
      }
    """

  * text variables =
    """
      {
        "code": "e74d9681647146478a3aa49223ec860b"
      }
    """

  Given path "/graphql"
  And request { query: '#(query)', operationName: "ContinentTest", variables: '#(variables)' }
  When method post
  Then status 200
  And match response.data.continent == "##(continentSchema)"

Scenario: Perform a countries query and validate the response
  * text query =
    """
      query CountriesTest {
        countries {
          code
          name
          native
          phone
          currency
          languages {
            code
            name
            native
            rtl
          }
          emoji
          emojiU
          states {
            code
            name
          }
        }
      }
    """

  Given path "/graphql"
  And request { query: '#(query)', operationName: "CountriesTest" }
  When method post
  Then status 200
  And match each response.data.countries == "##(countrySchema)"

Scenario: Perform a country query and validate the response
  * text query =
    """
      query CountryTest($code: ID!) {
        country(code: $code) {
          code
          name
          native
          phone
          currency
          languages {
            code
            name
            native
            rtl
          }
          emoji
          emojiU
          states {
            code
            name
          }
        }
      }
    """

  * text variables =
    """
      {
        "code": "6e6d80904cd14169849ded467880b477"
      }
    """

  Given path "/graphql"
  And request { query: '#(query)', operationName: "CountryTest", variables: '#(variables)' }
  When method post
  Then status 200
  And match response.data.country == "##(countrySchema)"

Scenario: Perform a languages query and validate the response
  * text query =
    """
      query LanguagesTest {
        languages {
          code
          name
          native
          rtl
        }
      }
    """

  Given path "/graphql"
  And request { query: '#(query)', operationName: "LanguagesTest" }
  When method post
  Then status 200
  And match each response.data.languages == "##(languageSchema)"

Scenario: Perform a language query and validate the response
  * text query =
    """
      query LanguageTest($code: ID!) {
        language(code: $code) {
          code
          name
          native
          rtl
        }
      }
    """

  * text variables =
    """
      {
        "code": "ee917129e9e349318c78a3b7f858711a"
      }
    """

  Given path "/graphql"
  And request { query: '#(query)', operationName: "LanguageTest", variables: '#(variables)' }
  When method post
  Then status 200
  And match response.data.language == "##(languageSchema)"