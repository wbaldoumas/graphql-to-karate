# graphql-to-karate 🚀

**Project Status**: v0.1.0 release. The project is still in its early stages. To report a bug, feel free to open an [issue](https://github.com/wbaldoumas/graphql-to-karate/issues).

[![Build][github-checks-shield]][github-checks-url]
[![Coverage][coverage-shield]][coverage-url]

[![Version][nuget-version-shield]][nuget-url]
[![Downloads][nuget-downloads-shield]][nuget-url]

[![Contributor Covenant][contributor-covenant-shield]][contributor-covenant-url]
[![Contributors][contributors-shield]][contributors-url]
[![Commits][last-commit-shield]][last-commit-url]

[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![MIT License][license-shield]][license-url]

[![LinkedIn][linkedin-shield]][linkedin-url]

## 🎯 About The Project

Automagically generate Karate API tests from your GraphQL schemas. Useful for test-driven development, change validation in CI/CD, and more.

## 📢 Demonstration

https://user-images.githubusercontent.com/45316999/233819219-5149e02d-649f-4c3d-a3fd-844a231cf515.mp4

### ✨ Features

Here are some features that this tool provides:

- **Generate API tests from GraphQL schemas:** Karate API test generation for both Query and Mutation operations is supported.
- **Filterable Query and Mutation operations:** Allows you to filter down and generate tests for a specific subset of target operations.
- **Validate response codes and response shapes:** Validates response codes and response schemas using Karate [schema validation](https://github.com/karatelabs/karate#schema-validation).
- **Handles cyclical type relationships:** Prunes cyclical types to remove cycles as they are encountered.
- **Supports custom scalar types:** Custom scalar types may be optionally mapped to their corresponding Karate schema types.
- **Generates test data for arguments and input types:** Test data for query arguments and input types is generated automatically.
- **Interactive and non-interactive CLI modes:** Supports both interactive and non-interactive CLI modes, as well as JSON configuration.

See the full list of available options below for more information.

## Installation

### 📥 Binary Releases

Head over to [releases](https://github.com/wbaldoumas/graphql-to-karate/releases) and download the latest binary for your specific platform. If a binary for your platform is not available, see the [Package Manager](#-package-manager) or [Building From Source](#-building-from-source) sections below.

### 📦 Package Manager

`graphql-to-karate` is also available as a [NuGet](https://www.nuget.org/packages/graphql-to-karate) offering and can be installed with the following command:

```sh
dotnet tool install --global graphql-to-karate --version 0.1.0
```

### 🛠️ Building From Source

To build from source, clone the repository locally and run some flavor of the following command. Be sure to update `<runtime identifier>` to your target platform. A catalog of available runtime identifiers can be viewed [here](https://learn.microsoft.com/en-us/dotnet/core/rid-catalog).

```sh
dotnet publish src/GraphQLToKarate.CommandLine/GraphQLToKarate.CommandLine.csproj \
  --configuration Release \
  --runtime <runtime identifier> \
  --output ./publish \
  --self-contained true \
  --force \
  /p:PublishReadyToRun=true \
  /p:PublishSingleFile=true \
  /p:TreatWarningsAsErrors=false
```

Once published, you can move the binary to your preferred install location to use.

## 🌌 Usage

To use `graphql-to-karate`, simply invoke the `graphql-to-karate convert` command, passing your GraphQL schema file as an argument:

```sh
graphql-to-karate convert my-schema.graphql
```

By default, you will be walked through conversion in an interactive way within the CLI.

A `--non-interactive` option as well as [JSON configuration](https://github.com/wbaldoumas/graphql-to-karate/blob/main/configuration/schema/v1/schema.json) are also available (see full [Command Options](#-convert-command-options) below), which may be useful in CI/CD environments.

### 📖 Examples of Non-Interactive Invokation

#### Using Command-Line Options

```sh
graphql-to-karate convert my-schema.graphql \
  --non-interactive \
  --base-url "https://my-api.com" \
  --custom-scalar-mapping DateTime:string,Long:number,Float:number,URL:string \
  --query-operation-filter Users,UserById \
  --output-file some-api.feature\
```

This flavor of `graphql-to-karate` usage is used within CI/CD validation for this repository, where a mock GraphQL server is spun up, `graphql-to-karate` converts the GraphQL schema to a Karate API test, and then the generated Karate API tests are run. Check it out [here](https://github.com/wbaldoumas/graphql-to-karate/blob/main/.github/workflows/integration-test.yml).

#### Using a JSON configuration

```sh
graphql-to-karate convert my-schema.graphql --non-interactive --configuration-file config.json
```

### 📝 Convert Command Options

| OPTION                      | DEFAULT                        | ABOUT                                                                                                                                      |
|-----------------------------|--------------------------------|--------------------------------------------------------------------------------------------------------------------------------------------|
| -h, --help                  |                                | Prints help information                                                                                                                    |
| --log-level                 | Information                    | Minimum level for logging                                                                                                                  |
| --non-interactive           | false                          | Whether to run conversion in a non-interactive way or not                                                                                  |
| --output-file               | graphql.feature                | The output file to write the Karate feature to                                                                                             |
| --query-name                | Query                          | The name of the GraphQL query type                                                                                                         |
| --mutation-name             | Mutation                       | The name of the GraphQL mutation type                                                                                                      |
| --exclude-queries           | false                          | Whether to exclude queries from the Karate feature or not                                                                                  |
| --include-mutations         | false                          | Whether to include mutations in the Karate feature or not                                                                                  |
| --base-url                  | "https://your-awesome-api.com" | The base URL to be used in the Karate feature                                                                                              |
| --custom-scalar-mapping     |                                | The path or raw value custom scalar mapping                                                                                                |
| --query-operation-filter    |                                | A comma-separated list of GraphQL query operations to include in the Karate feature. If empty, all query operations will be included       |
| --mutation-operation-filter |                                | A comma-separated list of GraphQL mutation operations to include in the Karate feature. If empty, all mutation operations will be included |
| --type-filter               |                                | A comma-separated list of GraphQL types to include in the Karate feature. If empty, all types will be included                             |
| --configuration-file        |                                | The path of the configuration file                                                                                                         |

## 🗺️ Roadmap

See the [open issues](https://github.com/wbaldoumas/graphql-to-karate/issues) for a list of proposed features (and known issues).

## 🤝 Contributing

Contributions are what make the open source community such an amazing place to learn, inspire, and create. Any contributions you make are **greatly appreciated**. For detailed contributing guidelines, please see the [CONTRIBUTING](https://github.com/wbaldoumas/graphql-to-karate/blob/main/CONTRIBUTING.md) docs.

## 📜 License

Distributed under the `MIT License` License. See [`LICENSE`](https://github.com/wbaldoumas/graphql-to-karate/blob/main/LICENSE) for more information.

## Contact

[@wbaldoumas](https://github.com/wbaldoumas)

Project Link: [https://github.com/wbaldoumas/graphql-to-karate](https://github.com/wbaldoumas/graphql-to-karate)

<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->
[contributors-shield]: https://img.shields.io/github/contributors/wbaldoumas/graphql-to-karate.svg?style=for-the-badge
[contributors-url]: https://github.com/wbaldoumas/graphql-to-karate/graphs/contributors
[contributor-covenant-shield]: https://img.shields.io/badge/Contributor%20Covenant-2.1-4baaaa.svg?style=for-the-badge
[contributor-covenant-url]: https://github.com/wbaldoumas/graphql-to-karate/blob/main/CODE_OF_CONDUCT.md
[forks-shield]: https://img.shields.io/github/forks/wbaldoumas/graphql-to-karate.svg?style=for-the-badge
[forks-url]: https://github.com/wbaldoumas/graphql-to-karate/network/members
[stars-shield]: https://img.shields.io/github/stars/wbaldoumas/graphql-to-karate.svg?style=for-the-badge
[stars-url]: https://github.com/wbaldoumas/graphql-to-karate/stargazers
[issues-shield]: https://img.shields.io/github/issues/wbaldoumas/graphql-to-karate.svg?style=for-the-badge
[issues-url]: https://github.com/wbaldoumas/graphql-to-karate/issues
[license-shield]: https://img.shields.io/github/license/wbaldoumas/graphql-to-karate.svg?style=for-the-badge
[license-url]: https://github.com/wbaldoumas/graphql-to-karate/blob/main/LICENSE
[linkedin-shield]: https://img.shields.io/badge/-LinkedIn-black.svg?style=for-the-badge&logo=linkedin&colorB=555
[linkedin-url]: https://linkedin.com/in/williambaldoumas
[coverage-shield]: https://img.shields.io/codecov/c/github/wbaldoumas/graphql-to-karate?style=for-the-badge
[coverage-url]: https://app.codecov.io/gh/wbaldoumas/graphql-to-karate/branch/main
[last-commit-shield]: https://img.shields.io/github/last-commit/wbaldoumas/graphql-to-karate?style=for-the-badge
[last-commit-url]: https://github.com/wbaldoumas/graphql-to-karate/commits/main
[github-checks-shield]: https://img.shields.io/github/actions/workflow/status/wbaldoumas/graphql-to-karate/test.yml?style=for-the-badge
[github-checks-url]: https://github.com/wbaldoumas/graphql-to-karate/actions
[nuget-version-shield]: https://img.shields.io/nuget/v/graphql-to-karate?style=for-the-badge
[nuget-downloads-shield]: https://img.shields.io/nuget/dt/graphql-to-karate?style=for-the-badge
[nuget-url]: https://www.nuget.org/packages/graphql-to-karate/
