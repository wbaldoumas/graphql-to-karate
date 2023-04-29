import sys

def main(changelog_file, output_file):
    with open(changelog_file, "r") as file:
        content = file.read()

    # Split the content by release sections
    sections = content.split("\n## ")
    latest_release = sections[1].strip()

    # Add the correct header values to the first line
    first_line, rest = latest_release.split("\n", 1)
    formatted_first_line = f"## {first_line}\n"

    # Combine the formatted first line and the rest of the release notes
    formatted_latest_release = formatted_first_line + rest

    # Write the latest release to a markdown file
    with open(output_file, "w") as file:
        file.write(formatted_latest_release)

if __name__ == "__main__":
    main(sys.argv[1], sys.argv[2])
