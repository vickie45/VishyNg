function convertDate(dateStr) {
  const [day, month, year] = dateStr.split('/');
  const monthNum = new Date(`${month} 1`).getMonth() + 1;
  return `${year}-${String(monthNum).padStart(2, '0')}-${day}`;
}

console.log(convertDate("07/Nov/2024")); // Output: "2024-11-07"