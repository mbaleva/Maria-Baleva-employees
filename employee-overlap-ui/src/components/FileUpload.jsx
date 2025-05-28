import React, { useState } from 'react';
import axios from 'axios';

const FileUpload = ({ onUploadSuccess }) => {
  const [file, setFile] = useState(null);
  const [uploading, setUploading] = useState(false);

  const handleChange = (e) => setFile(e.target.files[0]);

  const handleUpload = async () => {
    if (!file) return alert("Select a file first!");

    const formData = new FormData();
    formData.append('file', file);

    const apiBaseUrl =  import.meta.env.VITE_API_URL;

    try {
      setUploading(true);
      const response = await axios.post(`${apiBaseUrl}/api/files/upload`, formData);
      onUploadSuccess(response.data);
    } catch (err) {
      console.error(err);
      alert("Upload failed!");
    } finally {
      setUploading(false);
    }
  };

  return (
    <div className="mb-3">
      <input type="file" accept=".csv" onChange={handleChange} className="form-control" />
      <button
        onClick={handleUpload}
        disabled={uploading}
        className="btn btn-primary mt-2"
      >
        {uploading ? "Uploading..." : "Upload"}
      </button>
    </div>
  );
};

export default FileUpload;
