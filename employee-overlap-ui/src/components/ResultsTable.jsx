import React from 'react';

const ResultsTable = ({ results }) => {
  if (!results || results.length === 0) {
    return <p>No results to display.</p>;
  }

  const maxPair = results.reduce((max, curr) => 
    curr.daysWorked > max.daysWorked ? curr : max, results[0]);

  return (
    <table className="table table-striped table-hover">
      <thead className="table-dark">
        <tr>
          <th>Employee #1</th>
          <th>Employee #2</th>
          <th>Project ID</th>
          <th>Days Worked</th>
        </tr>
      </thead>
      <tbody>
        {results.map((item, index) => {
          const isMax = 
            item.empID1 === maxPair.empID1 &&
            item.empID2 === maxPair.empID2 &&
            item.projectID === maxPair.projectID;

          return (
            <tr key={index} className={isMax ? 'table-success' : ''}>
              <td>{item.empID1}</td>
              <td>{item.empID2}</td>
              <td>{item.projectID}</td>
              <td>{item.daysWorked}</td>
            </tr>
          );
        })}
      </tbody>
    </table>
  );
};

export default ResultsTable;
