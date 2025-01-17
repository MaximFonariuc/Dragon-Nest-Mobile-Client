import os
import zipfile
import sys

def extract_file(zip_path, extract_to):
    with zipfile.ZipFile(zip_path, 'r') as zipf:
        zipf.extractall(extract_to)

def join_files(file_base, output_file):
    part_num = 1
    with open(output_file, 'wb') as outfile:
        while True:
            part_file = f"{file_base}.part{part_num}"
            if not os.path.exists(part_file):
                break
            with open(part_file, 'rb') as infile:
                outfile.write(infile.read())
            os.remove(part_file)
            part_num += 1

def restore_files(directory):
    log_file = os.path.join(directory, "compression_log.txt")
    if not os.path.exists(log_file):
        print("No compression log found!")
        return

    try:
        with open(log_file, 'r') as log:
            for line in log:
                original_path, archive_path = line.strip().split('|')
                # Преобразуем относительные пути в абсолютные
                original_path = os.path.join(directory, original_path)
                archive_path = os.path.join(directory, archive_path)

                if archive_path.endswith('.part*'):
                    base_archive = archive_path[:-6]
                    print(f"Joining {base_archive} parts...")
                    join_files(base_archive, original_path)
                else:
                    print(f"Extracting {archive_path} to {original_path}...")
                    extract_file(archive_path, os.path.dirname(original_path))
                    os.remove(archive_path)

        os.remove(log_file)
    except Exception as e:
        print(f"An error occurred: {e}")

if __name__ == "__main__":
    if len(sys.argv) != 2:
        print("Usage: python RestoreLFS.py <directory>")
        sys.exit(1)

    directory_to_restore = sys.argv[1]
    if not os.path.isdir(directory_to_restore):
        print(f"Error: {directory_to_restore} is not a valid directory.")
        sys.exit(1)

    restore_files(directory_to_restore)
