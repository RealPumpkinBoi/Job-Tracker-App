import { Paper, Table, TableBody, TableCell, TableContainer, TableHead, TableRow } from '@mui/material';
import React, {useState, useEffect} from 'react';

interface DataItem {
    id: number;
    title: string;
    companyName: string;
    salary: string;
    resumeSent: Date | null; 
    followedUp: Date | null;
    interview1: Date | null;
    interview2: Date | null;
    interview3: Date | null;
    contactName: string;
    contactEmail: string;
    contactPhone: string;
    link: string;
}

const formatDate = (date: Date | null | undefined): string => {
    if (!date) {
        return 'N/A'; // Show 'N/A' if the date is null/missing
    }
    const dateObj = (typeof date === 'string') ? new Date(date) : date;
    if (isNaN(dateObj.getTime())) {
        return 'Invalid Date';
    }
    return dateObj.toLocaleDateString(); // Formats date (e.g., 9/27/2025)
};

const formatDateTime = (date: Date | null | undefined): string => {
    if (!date) {
        return 'N/A';
    }
    // Handle string inputs by converting them to Date objects
    const dateObj = (typeof date === 'string') ? new Date(date) : date;
    
    if (isNaN(dateObj.getTime())) {
        return 'Invalid Date';
    }
    
    // Formats both date and time (e.g., 9/27/2025, 4:30:00 PM)
    return dateObj.toLocaleString(); 
    
    // If you only wanted the time:
    // return dateObj.toLocaleTimeString(); // e.g., 4:30:00 PM
};

const JobDetail: React.FC = () => {
  const [data, setData] = useState<DataItem | null>(null);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
 
  // ----------------------------------------------------
  // 2. Data Fetching Logic using useEffect and Fetch API
  // ----------------------------------------------------
  useEffect(() => {
    const API_URL = 'api/detail/2020';


    const fetchData = async () => {
      // Start loading
      setLoading(true);
      setError(null);


      try {
        const response = await fetch(API_URL);
       
        // Throw an error if the HTTP status is not successful (e.g., 404, 500)
        if (!response.ok) {
          throw new Error(`HTTP error! status: ${response.status}`);
        }
    
        // Parse the JSON response
        const result: DataItem = await response.json();
        console.log(result);
        setData(result); // Store the fetched data
       
      } catch (e) {
        // Handle network errors or non-200 responses
        console.error("Failed to fetch data:", e);
        // Cast 'e' to Error to safely access its message
        setError(`Failed to load data: ${e instanceof Error ? e.message : 'Unknown error'}`);
      } finally {
        // Stop loading regardless of success or failure
        setLoading(false);
      }
    };


    fetchData();
   // The empty dependency array [] ensures this effect runs only once after the initial render (componentDidMount equivalent)
  }, []);


  // ----------------------------------------------------
  // 3. Conditional Rendering based on fetch state
  // ----------------------------------------------------
  let content;


  if (loading) {
    content = (
      <div className="flex items-center justify-center space-x-2 h-48 bg-indigo-50/50 rounded-lg">
        {/* Simple Loading Spinner */}
        <svg className="animate-spin h-5 w-5 text-indigo-500" viewBox="0 0 24 24">
          <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
          <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
        </svg>
        <p className="text-indigo-600 font-medium">Loading Data...</p>
      </div>
    );
  } else if (error) {
    content = (
      <div className="p-4 bg-red-100 border border-red-400 text-red-700 rounded-lg h-48 flex items-center justify-center">
        <p className="font-semibold">{error}</p>
      </div>
    );
  } else if (data) {
    content = (
    <TableContainer component={Paper}>
      <Table sx={{ minWidth: 650 }} aria-label="simple table">
        <TableHead>
          <TableRow>
            <TableCell>Title</TableCell>
            <TableCell>CompanyName</TableCell>
            <TableCell>Salary</TableCell>
            <TableCell>ResumeSent</TableCell>
            <TableCell>FollowedUp</TableCell>
            <TableCell>Interview 1</TableCell>
            <TableCell>Interview 2</TableCell>
            <TableCell>Interview 3</TableCell>
            <TableCell>ContactName</TableCell>
            <TableCell>ContactEmail</TableCell>
            <TableCell>ContactPhone</TableCell>
            <TableCell>Link</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
            <TableRow
              key={data.id}
              sx={{ '&:last-child td, &:last-child th': { border: 0 } }}
            >
              <TableCell component="th" scope="row">
                {data.title}
              </TableCell>
              <TableCell>{data.companyName}</TableCell>
              <TableCell>{data.salary}</TableCell>
              <TableCell>{formatDate(data.resumeSent)}</TableCell>
              <TableCell>{formatDate(data.followedUp)}</TableCell>
              <TableCell>{formatDateTime(data.interview1)}</TableCell>
              <TableCell>{formatDateTime(data.interview2)}</TableCell>
              <TableCell>{formatDateTime(data.interview3)}</TableCell>
              <TableCell>{data.contactName}</TableCell>
              <TableCell>{data.contactEmail}</TableCell>
              <TableCell>{data.contactPhone}</TableCell>
              <TableCell>{data.link}</TableCell>
            </TableRow>
        </TableBody>
      </Table>
    </TableContainer>

    );
  } else {
     content = (
      <div className="h-48 border-2 border-dashed border-indigo-300 rounded-lg flex items-center justify-center bg-indigo-50/50">
          <p className="text-indigo-400 font-medium italic">
            No data retrieved.
          </p>
        </div>
     );
  }

  return content;
};

export default JobDetail;