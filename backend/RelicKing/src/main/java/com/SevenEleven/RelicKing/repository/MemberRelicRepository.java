package com.SevenEleven.RelicKing.repository;

import java.util.List;

import org.springframework.data.jpa.repository.JpaRepository;

import com.SevenEleven.RelicKing.entity.Member;
import com.SevenEleven.RelicKing.entity.MemberRelic;

public interface MemberRelicRepository extends JpaRepository<MemberRelic, Integer> {

	List<MemberRelic> findByMember(Member member);

}
