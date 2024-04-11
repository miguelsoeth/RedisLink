const axios = require('axios');

// Function to make a POST request to the specified URL with the given data
async function insertData(url, data) {
  try {
    const response = await axios.post(url, data, { 
      httpsAgent: new (require("https").Agent)({ rejectUnauthorized: false })
    });
    console.log(response.data);
  } catch (error) {
    console.error('Error:', error);
  }
}

// Function to generate and post data in the specified pattern
async function postMultipleData(url, count) {
  for (let i = 1; i <= count; i++) {
    const postData = {
      id: i,
      name: `Driver ${i}`,
      driverNb: i * 3
    };
    await insertData(url, postData); // Fixed to call postData function
  }
}

// Define the URL
const apiUrl = 'https://host.docker.internal:32781/Drivers/drivers/add';

// Make 100 posts with the specified pattern
postMultipleData(apiUrl, 2000);
