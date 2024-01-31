import React from 'react';
import {
  Card,
  CardActionArea,
  CardContent,
  CardMedia,
  Typography,
  Avatar,
  Grid
} from '@mui/material';
import { useNavigate } from 'react-router-dom';

export default function VideoCard({props}) {
  const navigate = useNavigate()
  function handleClick()
  {
    navigate('/player/videoId')
  }
  return (
    <Grid item xs={12} sm={6} md={4}>
      <Card
        sx={{
          maxWidth: 345,
          transition: 'transform 0.2s ease-in-out',
          ':hover': {
            transform: 'scale(1.05)',
          },
        }} >
        <CardActionArea onClick={(ev)=>handleClick()}>
          <CardMedia
            component="img"
            height="200"
            image="https://via.placeholder.com/345x200"
            alt="Video Thumbnail"
          />
          <CardContent sx={{height:'100px',padding:'5px',paddingLeft:'20px'}}>
            <Typography variant="h6" component="div" >
              Video Title
            </Typography>
            <div style={{ display: 'flex', alignItems: 'center' }}>
              <Avatar
                src="https://via.placeholder.com/30x30"
                alt="User Avatar"
                sx={{ width: 30, height: 30, marginRight: 1 }}
              />
              <Typography variant="subtitle2">Username</Typography>
            </div>
            <Typography variant="subtitle2" color="textSecondary" sx={{marginLeft:'20px'}}>
              1.5k views
            </Typography>
          </CardContent>
        </CardActionArea>
      </Card>
    </Grid>
  );
};