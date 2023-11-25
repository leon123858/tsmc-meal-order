const fs = require('fs');

const uploadImage = async () => {
    try {
        // const url = 'http://localhost:8080/api/storage/upload/image';
        const url = 'https://storage-kt6w747drq-de.a.run.app/api/storage/upload/image'
        const imagePath = './script/test.jpg';

        // Read the image file
        const image = fs.readFileSync(imagePath);
        // Convert image file to Blob
        const imageBlob = new Blob([image], { type: 'image/jpeg' });

        // Create FormData
        const formData = new FormData();
        formData.set('image', imageBlob, Math.random() + '.jpg');

        const JWT = "jwt token now"

        // Make a POST request with fetch
        const response = await fetch(url, {
            method: 'POST',
            body: formData,
            headers: {
                "Authorization": "Bearer " + JWT
            },
        });

        const data = await response.text();
        console.log("data is file's public url");
        console.log(data);
    } catch (error) {
        console.error('Error uploading image:', error.message);
    }
};

uploadImage();