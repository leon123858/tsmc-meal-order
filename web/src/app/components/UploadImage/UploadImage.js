import {
	CheckCircleOutlined,
	CloudUploadOutlined,
	FolderOpenFilled,
} from '@ant-design/icons';
import { Button, Modal, Spin, Upload, Image } from 'antd';
import { useState, useEffect } from 'react';	
import ImgCrop from 'antd-img-crop';
import { ImageAPI } from '../../global';
import { auth } from '../../firebase';

const uploadImageToCloud = async (image, index) => {
	try {
		const url = ImageAPI;
		// Create FormData
		const imageBlob = new Blob([image], { type: 'image/jpeg' });
		const formData = new FormData();
		formData.set('image', imageBlob, index + '.jpg');

		// can get jwt from firebase SDK in client
		let JWT = "";
		await auth.currentUser.getIdToken().then((idToken) => {
			JWT = idToken;
		}).catch((error) => {
			console.error('發生錯誤:', error);
		})

		// Make a POST request with fetch
		const response = await fetch(url, {
			method: 'POST',
			body: formData,
			headers: {
				"Authorization": "Bearer " + JWT
			},
		});

		const data = await response.json();
		console.log("data is file's public url");
		console.log(data);
		return data.data;
	} catch (error) {
		console.error('Error uploading image:', error.message);
	}
}

export const UploadImage = ({setUrl, _isUpload, text, index}) => {
	const [previewVisible, setPreviewVisible] = useState(false);
	const [previewImage, setPreviewImage] = useState('');
	const [previewTitle, setPreviewTitle] = useState('');
	const [imageFile, setImageFile] = useState(null);
	const [isUpload, setisUpload] = useState(_isUpload ?? false);
	const [loading, setloading] = useState(false);
	const handleCancel = () => setPreviewVisible(false);

	useEffect(() => {
		if (_isUpload !== undefined && !_isUpload) {
			setisUpload(false);
		}
	}, [_isUpload]);

	const uploadButton = (
		<div>
			<FolderOpenFilled />
			<div style={{ marginTop: 8 }}>瀏覽本機圖片</div>
		</div>
	);

	const localPreview = async (options) => {
		let isOK = true;
		const file = options.file;
		const onSuccess = options.onSuccess;
		const isLt2M = file.size > 25 * 1024 * 1024;

		const isJpgOrPng = file.type === 'image/jpeg' || file.type === 'image/png';
		if (!isJpgOrPng) {
			alert('You can only upload JPG/PNG file!');
			isOK = false;
		} else if (isLt2M) {
			alert('Image must smaller than 25 MB!');
			isOK = false;
		}

		if (isOK) {
			const url = URL.createObjectURL(file);
			setImageFile(file);
			setPreviewImage(url);
			setPreviewTitle((options.file).name);
			setisUpload(false);
			onSuccess();
		} else {
			setisUpload(false);
		}
	};

	const uploadImage = async () => {
		const url = await uploadImageToCloud(imageFile, index);
		if (url !== null && url !== undefined) {
			URL.revokeObjectURL(previewImage);
			setPreviewImage(url);
			setPreviewTitle('已上傳');
			setisUpload(true);
			setUrl(url);
		}
	};
	return (
		<>
            <ImgCrop showGrid rotationSlider aspectSlider showReset>
                <Upload
					name="foodImage"
                    accept='image/*'
                    style={{ width: 230 }}
                    customRequest={localPreview}
                    listType='picture'
                    onPreview={() => setPreviewVisible(true)}
                >
                    {uploadButton}
                </Upload>
            </ImgCrop>

			<Modal
				open={previewVisible}
				title={previewTitle}
				footer={null}
				onCancel={handleCancel}
			>
				<Image alt='upload' style={{ width: '100%' }} src={previewImage} />
			</Modal>
			<Spin tip='Loading...' spinning={loading}>
				<Button
					type='primary'
					icon={!isUpload ? <CloudUploadOutlined /> : <CheckCircleOutlined />}
					style={{ width: 150, marginTop: 15 }}
					onClick={async () => {
						if (previewImage) {
							try {
								setloading(true);
								await uploadImage();
								if (_isUpload === undefined) {
									setisUpload(false);
								}
							} catch (error) {
								alert('上傳失敗');
							} finally {
								setloading(false);
							}
						} else {
							alert('請選擇圖片');
						}
					}}
					disabled={isUpload}
				>
					{text ? text : isUpload ? '已更新' : '更新'}
				</Button>
			</Spin>
		</>
	);
};