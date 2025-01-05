import React, { createRef, RefObject } from 'react';
import Papa from 'papaparse';

export default class UploadButton extends React.Component {
  fileInputRef: RefObject<HTMLInputElement | null>;

  constructor(props: any) {
    super(props);
    this.fileInputRef = createRef<HTMLInputElement>();
  }

  state = {
    isLoading: false, // Track loading state
  };

  verifyFile = async (file: File): Promise<string | null> => {
    const MAX_FILE_SIZE_MB = 10; // Maximum file size in MB
    const MAX_LINE_LENGTH = 100; // Maximum length of a single line

    // Check file size
    const fileSizeMB : number = file.size / (1024 * 1024);
    if (fileSizeMB > MAX_FILE_SIZE_MB) {
      return `File size exceeds the maximum limit of ${MAX_FILE_SIZE_MB} MB.`;
    }

    // Read file content to check line lengths
    const fileContent = await file.text();
    const lines = fileContent.split('\n').map(line => line.trim());
    const longLines = lines.filter(line => line.length > MAX_LINE_LENGTH);

    if (longLines.length > 0) {
      return `The file contains lines exceeding the maximum allowed length of ${MAX_LINE_LENGTH} characters.`;
    }

    return null; // No errors
  };

  downloadCsv = (csv: string, filename: string) => {
    const blob = new Blob([csv], { type: 'text/csv' });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = filename;
    a.click();
    URL.revokeObjectURL(url);
  };

  handleFileChange = async (event: React.ChangeEvent<HTMLInputElement>) => {
    this.setState({ isLoading: true }); // Set loading to true

    const file = event.target.files?.[0];
    if (file) {
      // Validate the file
      const error = await this.verifyFile(file);
      if (error) {
        alert(error);
        event.target.value = ''; // Reset file input
        return;
      }

      const reader = new FileReader();
  
      reader.onload = async () => {
        const fileContent : string = reader.result as string;
        const gameNames : string [] = fileContent.split('\n').map(line => line.trim()).filter(line => line.length > 0);


        const chunkSize = 10; // Number of games per batch
        let allData: JSON[] = []; // Array to accumulate all responses

        // Loop through game names in chunks
        for (let i = 0; i < gameNames.length; i += chunkSize) {
          const chunk = gameNames.slice(i, i + chunkSize);
          const url = `https://localhost:7107/GameInfo/games?${chunk.map(game => `gameNames=${encodeURIComponent(game)}`).join('&')}`;
          
          try {
            const response: Response = await fetch(url, {
              method: 'GET',
            });
            const data: JSON = await response.json();
            allData = allData.concat(data); // Append the fetched data to the allData array
          } catch (error) {
            console.error(`Error fetching data for chunk starting at index ${i}:`, error);
          }
        }

        // Convert JSON to CSV using json2csv
        const csv = Papa.unparse(allData);
        const fileNameWithoutExtension = file.name.slice(0, -file.name.split('.').pop()!.length - 1);
        this.downloadCsv(csv, `${fileNameWithoutExtension}.csv`);

        this.setState({ isLoading: false });
      };
  
      reader.readAsText(file);
    }
    // Reset the input value
    event.target.value = '';
  };

  handleButtonClick = () => {
    if (this.fileInputRef.current) {
      this.fileInputRef.current.click();
    }
  };

  render(): React.JSX.Element {
    const { isLoading } = this.state;
    return (
      <div>
        <input
          type="file"
          ref={this.fileInputRef}
          style={{ display: 'none' }}
          onChange={this.handleFileChange}
        />
        <button
          className="button-upload"
          onClick={this.handleButtonClick}
        >
          {isLoading ? (
            <div className="loading-spinner"></div>
          ) : (
            'Upload'
          )}
        </button>
      </div>
    );
  }
}
