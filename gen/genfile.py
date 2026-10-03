import os
import json

def generate_project(data_file="project_data.json"):
    if not os.path.exists(data_file):
        print(f"Chyba: Datový soubor {data_file} nebyl nalezen.")
        return

    with open(data_file, "r") as f:
        project_files = json.load(f)

    for path, data in project_files.items():
        os.makedirs(os.path.dirname(path), exist_ok=True)
        
        # Sestavení obsahu s hlavičkou
        header_lines = [f"// {line}" for line in data["header"].split('\n')]
        full_content = "\n".join(header_lines) + "\n\n" + data["content"]
        
        with open(path, "w", encoding="utf-8") as f:
            f.write(full_content)
        print(f"Generuji: {path}")

if __name__ == "__main__":
    generate_project()
    print("\nProjekt byl úspěšně vygenerován z datového souboru.")