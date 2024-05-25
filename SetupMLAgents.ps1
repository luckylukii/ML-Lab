param (
    [string]$targetPath
)

# Check if a path was provided
if (-not $targetPath) {
    Write-Host "Error: Please provide a valid target path."
    exit 1
}

# Step 1: Create a Python Virtual Environment
Write-Host "Creating venv..."
cd $targetPath
python -m venv venv

# Step 2: Activate the Virtual Environment
Write-Host "Acitvating venv..."
venv\Scripts\activate

# Step 3: Install Unity ML-Agents and Dependencies
Write-Host "Installing dependencies..."
python -m pip install --upgrade pip
pip install torch torchvision torchaudio
pip install six
pip install mlagents
pip install importlib-metadata==4.4

# Fix error with tensorboard visualization
pip install urllib3==1.26.5

# Deactivate venv
venv\Scripts\deactivate

Write-Host "Setup finished. Here are the parameters of the 'mlagents-learn' command:"
mlagents-learn --help
