import React, { useContext } from 'react';
import { Container, Paper, TextField, Button, Typography, Input } from '@mui/material';
import { useNavigate } from 'react-router-dom';
import { useState } from 'react';
import { videoController } from '../Constants';
import UserContext from './Contexts/UserContext';

export default function VideoUploadForm() {
    const navigate = useNavigate()
    const [formState, setFormState] = useState(
        {
            Title:'',Description:'',Tags:'',Files:[]
        }
    )
    const [userState,setUserState] = useContext(UserContext)
  // Handle form submission
    const handleSubmit = async (event) => {
        function objectToFormData(obj) {
            const formData = new FormData();
          
            for (const key in obj) {
              if (obj.hasOwnProperty(key)) {
                if (Array.isArray(obj[key])) {
                  // If the property is an array, append each item individually
                  obj[key].forEach((item, index) => {
                    formData.append(`${key}`, item);
                  });
                } else {
                  formData.append(key, obj[key]);
                }
              }
            }
          
            return formData;
          }         
    event.preventDefault();
      // Handle video upload logic
      const body = {
          ...formState,
          Tags: formState.Tags.split(' '),
          ChannelName: userState.username,
          ChannelId:userState.id
      }
      if (body.Tags.filter(item => item == 'video').length== 0)
      {
          body.Tags.push('video')
      }
      
      const formData = objectToFormData(body)
      const response = await fetch(videoController + '/PostVideo', {
          method: 'POST',
          body:formData
      })
      if(response.ok)
          navigate('/')
      else
          alert("Error uploading video: "+await response.text())
    };

    function handleOnChange(ev)
    {
        setFormState(
            oldValue => {
                return {
                    ...oldValue,
                    [ev.target.name]:ev.target.value
                }
            }
        )
    }

    async function handleVideoUpload(event)
    {
        const file = event.target.files[0];
    
        // Read video file as a byte array
        const video = file
    
        // Create thumbnail as the first frame
        const thumbnailByteArray = await createThumbnail(file);
        const thumbnail =  new Blob([
            thumbnailByteArray],
            {
                type: 'image/jpeg',
            size:thumbnailByteArray.length}
        )
    
        // Set state with video and thumbnail data
        setFormState(oldValue =>
        {
            return {
                ...oldValue,
                Files: [
                    video,thumbnail
                   
                ]
            }
            })
      };
    


  return (
    <Container component="main" maxWidth="xs">
      <Paper elevation={3} sx={{ padding: 3, display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
        <Typography component="h1" variant="h5">
          Upload Video
        </Typography>
        <form onSubmit={handleSubmit} style={{ width: '100%', marginTop: 1 }}>
          <TextField
            fullWidth
            margin="normal"
            required
            label="Title"
            name="Title"
                      variant="outlined"
                      value={formState.Title}
                      onChange={handleOnChange}
          />
          <TextField
            fullWidth
            margin="normal"
            required
            multiline
            rows={4}
            label="Description"
            name="Description"
                      variant="outlined"
                      value={formState.Description}
                      onChange={handleOnChange}
          />
          <TextField
            fullWidth
            margin="normal"
            label="Tags"
            name="Tags"
                      variant="outlined"
                      value={formState.Tags}
                      onChange={handleOnChange}
          />
          <Input
            fullWidth
            margin="normal"
            required
            type="file"
            inputProps={{ accept: 'video/*' }}
                      sx={{ display: 'none' }}
                      onChange={handleVideoUpload}
                      id='video-upload'
                      name='video'
          />
          <label htmlFor="video-upload"  onChange={handleVideoUpload}>
            <Button
              variant="outlined"
              component="span"
              fullWidth
                          sx={{ marginTop: 2 }}
                          onChange={handleVideoUpload}
            >
              Upload Video
            </Button>
          </label>
          <Button
            type="submit"
            fullWidth
            variant="contained"
            sx={{ mt: 2 }}
          >
            Submit
          </Button>
        </form>
      </Paper>
    </Container>
  );
};


  

const createThumbnail = async (file) => {
    return new Promise((resolve) => {
      const video = document.createElement('video');
      video.crossOrigin = 'anonymous';
  
      const canvas = document.createElement('canvas');
      const context = canvas.getContext('2d');
  
      video.addEventListener('loadeddata', () => {
        // Set canvas dimensions based on video dimensions
        canvas.width = video.videoWidth;
        canvas.height = video.videoHeight;

        // Seek to the 1st second
        video.currentTime = 1;
      });
  
      video.addEventListener('seeked', () => {
        // Draw the frame at the 1st second onto the canvas
        context.drawImage(video, 0, 0, canvas.width, canvas.height);
  
        // Convert canvas content to a data URL
        const dataURL = canvas.toDataURL('image/jpeg');
  
        // Convert data URL to a byte array
        const byteString = atob(dataURL.split(',')[1]);
        const byteArray = new Uint8Array(byteString.length);
        for (let i = 0; i < byteString.length; i++) {
          byteArray[i] = byteString.charCodeAt(i);
        }
        resolve(byteArray);
      });
  
      // Set the video source after adding the event listeners
      video.src = URL.createObjectURL(file);
      video.load();
    });
};

