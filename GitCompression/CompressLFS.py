import os
import zipfile
import sys

def compress_file(file_path, output_zip):
    """Сжимает файл в ZIP-архив."""
    with zipfile.ZipFile(output_zip, 'w', zipfile.ZIP_DEFLATED) as zipf:
        zipf.write(file_path, os.path.basename(file_path))

def split_file(file_path, max_size):
    """Разделяет файл на части указанного размера."""
    part_num = 1
    with open(file_path, 'rb') as f:
        while chunk := f.read(max_size):
            part_file = f"{file_path}.part{part_num}"
            with open(part_file, 'wb') as part:
                part.write(chunk)
            part_num += 1

def process_files(directory, max_size=50*1024*1024, split_size=49*1024*1024):
    """
    Обрабатывает файлы в папке Assets и ее подпапках.
    Сжимает файлы, превышающие max_size, и разделяет их, если это необходимо.
    """
    log_file = os.path.join(directory, "compression_log.txt")
    assets_folder = os.path.join(directory, "Assets")  # Обрабатываем только папку Assets

    if not os.path.exists(assets_folder):
        print(f"Error: 'Assets' folder not found in {directory}.")
        return

    try:
        with open(log_file, 'w') as log:
            for root, dirs, files in os.walk(assets_folder):
                # Игнорируем ненужные папки
                if '.git' in dirs:
                    dirs.remove('.git')
                if 'Library' in dirs:
                    dirs.remove('Library')
                if 'Logs' in dirs:
                    dirs.remove('Logs')

                for file in files:
                    file_path = os.path.join(root, file)
                    file_size = os.path.getsize(file_path)

                    if file_size > max_size:
                        print(f"Compressing {file_path}...")
                        zip_file = f"{file_path}.zip"
                        compress_file(file_path, zip_file)

                        zip_size = os.path.getsize(zip_file)
                        if zip_size > max_size:
                            print(f"File {file_path} is still too large after compression. Splitting...")
                            split_file(zip_file, split_size)
                            os.remove(zip_file)
                            # Записываем относительные пути
                            relative_file_path = os.path.relpath(file_path, directory)
                            relative_zip_file = os.path.relpath(zip_file, directory)
                            log.write(f"{relative_file_path}|{relative_zip_file}.part*\n")
                        else:
                            # Записываем относительные пути
                            relative_file_path = os.path.relpath(file_path, directory)
                            relative_zip_file = os.path.relpath(zip_file, directory)
                            log.write(f"{relative_file_path}|{relative_zip_file}\n")

                        os.remove(file_path)
    except FileNotFoundError as e:
        print(f"Error: {e}. Please check the directory path.")
    except Exception as e:
        print(f"An error occurred: {e}")

if __name__ == "__main__":
    if len(sys.argv) != 2:
        print("Usage: python CompressLFS.py <directory>")
        sys.exit(1)

    directory_to_process = sys.argv[1]
    if not os.path.isdir(directory_to_process):
        print(f"Error: {directory_to_process} is not a valid directory.")
        sys.exit(1)

    process_files(directory_to_process)
