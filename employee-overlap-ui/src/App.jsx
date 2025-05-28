import React, { useState } from 'react';
import FileUpload from './components/FileUpload';
import ResultsTable from './components/ResultsTable';

const App = () => {
  const [results, setResults] = useState([]);

  return (
    <div style={{ padding: '2rem' }}>
      <h1>Employee Overlap Tracker</h1>
      <FileUpload onUploadSuccess={setResults} />
      <br />
      <ResultsTable results={results} />
    </div>
  );
};

export default App;
