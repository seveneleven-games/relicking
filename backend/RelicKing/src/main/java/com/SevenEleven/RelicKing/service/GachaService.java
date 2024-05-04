package com.SevenEleven.RelicKing.service;

import java.util.Map;

import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import com.SevenEleven.RelicKing.entity.Member;

import lombok.RequiredArgsConstructor;
import lombok.extern.log4j.Log4j2;

@Service
@RequiredArgsConstructor
@Log4j2
@Transactional
public class GachaService {

	public Map<String, Integer> getGachaInfo(Member member) {
		return Map.of("gacha", member.getGacha());
	}
}
