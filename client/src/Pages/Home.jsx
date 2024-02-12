import React from 'react';
import {
  Grid,
  Container
} from '@mui/material';
import { useLoaderData } from 'react-router-dom';
import VideoCard from '../GeneralComponents/VideoCard';
import { videoController } from '../Constants';

export async function HomeLoader({params})
{
  const response = await fetch(videoController + '/GetVideos?skip=0&limit=20')
  if (response.ok)
  {
    const array = await response.json();
    return array
  }
  const array = /* new Array(33).fill(1) */[]
  return array
}

export default function Home() {
    const data = useLoaderData()
  const display = data.map(item => <VideoCard props={item} />)
    return (
      <Container sx={{marginTop:'110px'}}>
        <Grid container spacing={3}>
            {React.Children.map(display, (child, index) => (
            <React.Fragment key={index}>{child}</React.Fragment>
            ))}
      </Grid>
      </Container>
    );
  };