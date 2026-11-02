set -euo pipefail

# Usage: ./amplify.sh <directory> <gain_in_db>
DIR="${1:-.}"
GAIN="${2:?Usage: $0 [directory] <gain_in_db>}"

# Check dependencies
if ! command -v ffmpeg >/dev/null 2>&1; then
    echo "Error: ffmpeg is not installed."
    exit 1
fi

shopt -s nullglob nocaseglob

files=("$DIR"/*.wav)

if [[ ${#files[@]} -eq 0 ]]; then
    echo "No WAV files found in: $DIR"
    exit 0
fi

for file in "${files[@]}"; do
    echo "Amplifying: $file (+${GAIN} dB)"

    tmp="${file%.*}.tmp.wav"

    # Apply gain, preserving the WAV format
    ffmpeg -hide_banner -loglevel error -y \
        -i "$file" \
        -af "volume=${GAIN}dB" \
        -c:a pcm_s16le \
        "$tmp"

    # Replace original file
    mv "$tmp" "$file"
done

echo "Done! Processed ${#files[@]} WAV files."